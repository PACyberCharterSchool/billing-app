using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Swashbuckle.AspNetCore.SwaggerGen;

using api.Common;
using api.Common.Utils;
using api.Dtos;
using models;
using models.Reporters;
using models.Reporters.Exporters;

using Aspose.Cells;

namespace api.Controllers
{
  [Route("api/[controller]")]
  public class ReportsController : Controller
  {
    private readonly PacBillContext _context;
    private readonly IReportRepository _reports;
    private readonly IReporterFactory _reporters;
    private readonly ITemplateRepository _templates;
    private readonly IXlsxExporter _exporter;
    private readonly ISchoolDistrictRepository _districts;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
      PacBillContext context,
      IReportRepository reports,
      IReporterFactory reporters,
      ITemplateRepository templates,
      IXlsxExporter exporter,
      ISchoolDistrictRepository districts,
      ILogger<ReportsController> logger)
    {
      _context = context;
      _reports = reports;
      _reporters = reporters;
      _templates = templates;
      _exporter = exporter;
      _districts = districts;
      _logger = logger;
    }

    public class CreateInvoiceReport
    {
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

      [Required]
      public DateTime AsOf { get; set; }

      [Required]
      public DateTime ToSchoolDistrict { get; set; }

      [Required]
      public DateTime ToPDE { get; set; }

      [Required]
      [Range(100000000, 999999999)]
      public int SchoolDistrictAun { get; set; }
    }

    public class CreateBulkInvoiceReport
    {
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

			public DateTime AsOf { get; set; }

      public DateTime ToSchoolDistrict { get; set; }

      public DateTime ToPDE { get; set; }

      public bool Approved { get; set; }
    }

    public class CreateReport
    {
			[EnumerationValidation(typeof(ReportType))]
			[Required]
			public string ReportType { get; set; }

			[Required]
			[MinLength(1)]
			public string Name { get; set; }

			[Required]
			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }

			[Required]
			[Range(1, int.MaxValue)]
			public int TemplateId { get; set; }

			public CreateInvoiceReport Invoice { get; set; }

			public CreateBulkInvoiceReport BulkInvoice { get; set; }
    }

    private void CloneInvoiceSummarySheet(Workbook wb, int districtIndex)
    {
      wb.Worksheets.AddCopy(0);
      var sheet = wb.Worksheets[wb.Worksheets.Count - 1];
      Cells cells = sheet.Cells;
      for (int r = 0; r < cells.MaxDataRow + 1; r++) {
        for (int c = 0; c < cells.MaxDataColumn + 1; c++) {
          if (!(cells[r, c].Type == CellValueType.IsString))
            continue;
          const string pattern = @"Districts\[(\d+)\]";
          var matches = Regex.Matches(cells[r, c].StringValue, pattern);
          if (matches.Count > 0)
          {
            var match = matches[0];
            var i = int.Parse(match.Groups[1].Value);

            cells[r, c].Value = Regex.Replace(cells[r, c].StringValue, pattern, $"Districts[{districtIndex}]");
            cells[r, c].PutValue(cells[r, c].StringValue);
          }
        }
      }
    }

    private void CloneStudentItemizationSheets(Workbook wb, int count, int districtIndex, Template template)
    {
      const int per = 8;

      var numSheets = (int)count / per + (count % per == 0 ? 0 : 1);
      var adj = districtIndex == 0 ? 1 : 0;

      for (var s = 0; s < numSheets - adj; s++)
      {
        wb.Worksheets.AddCopy(1);

        var sheet = wb.Worksheets[wb.Worksheets.Count - 1];
        sheet.Name = $"Individual Student Inform. {s+1}";
        Cells cells = sheet.Cells;
        
        sheet.PageSetup.HeaderMargin = 0.0;
        sheet.PageSetup.FooterMargin = 0.0;
        sheet.PageSetup.BottomMargin = 0.0;
        sheet.PageSetup.TopMargin = 0.0;
        sheet.PageSetup.LeftMargin = 0.0;
        sheet.PageSetup.RightMargin = 0.0;
        sheet.PageSetup.HeaderMargin = 0.0;
        sheet.PageSetup.FooterMargin = 0.0;

        for (int r = 0; r < cells.MaxDataRow + 1; r++) {
          for (int c = 0; c < cells.MaxDataColumn + 1; c++) {
            if (!(cells[r, c].Type == CellValueType.IsString))
              continue;

            if (r == GetRowIndexForFirstStudentItemization(template) && c == 1) // Number column
            {
              cells[r,c].PutValue(((s + adj) * per) + 1);
              continue;
            }

            {
              const string pattern = @"Students\[(\d+)\]";
              var matches = Regex.Matches(cells[r,c].StringValue, pattern);
              if (matches.Count > 0)
              {
                var match = matches[0];
                var i = int.Parse(match.Groups[1].Value);

                cells[r,c].Value = Regex.Replace(cells[r,c].StringValue, pattern, $"Students[{(i + ((s + adj) * per))}]");
                cells[r,c].PutValue(cells[r,c].StringValue);
              }
            }

            {
              const string pattern = @"Districts\[(\d+)\]";
              var matches = Regex.Matches(cells[r,c].StringValue, pattern);
              if (matches.Count > 0)
              {
                var match = matches[0];
                var i = int.Parse(match.Groups[1].Value);

                cells[r,c].Value = Regex.Replace(cells[r,c].StringValue, pattern, $"Districts[{districtIndex}]");
                cells[r,c].PutValue(cells[r,c].StringValue);
              }
            }
          }
        }
      }
    }

    private int GetRowIndexForFirstStudentItemization(Template template)
    {
      int sequenceNumberRow;

      // What does this do you ask? Well, since we can't anticipate what the Pennsylvania
      // Department of Education is ever going to do with the invoice templates they provide
      // for use to cyber charter schools, we have to build logic in to find the cell that 
      // has the index value for the first student in the student itemization.  The remainder
      // of the index values are calculated from the first index cell.
      switch (template.SchoolYear)
      {
        case "2017 - 2018":
          if (template.ReportType == ReportType.Invoice)
            sequenceNumberRow = 13;
          else
            sequenceNumberRow = 13;
          break;
        case "2018 - 2019":
          if (template.ReportType == ReportType.Invoice)
            sequenceNumberRow = 12;
          else
            sequenceNumberRow = 12;
          break;
        default:
          sequenceNumberRow = 13;
          break;
      }

      return sequenceNumberRow;
    }

    private void CloneInvoiceSheets(Workbook wb, int count, Template template)
    {
      const int per = 8;

      var numSheets = (int)count / per + (count % per == 0 ? 0 : 1);
      for (var s = 0; s < numSheets - 1; s++)
      {
        wb.Worksheets.AddCopy(1);

        var sheet = wb.Worksheets[wb.Worksheets.Count - 1];
        Cells cells = sheet.Cells;
        sheet.Name = $"Individual Student Inform. {s+1}";

        for (int r = 0; r < cells.MaxDataRow + 1; r++) {
					Row row = cells.Rows[r];
					if (row.IsBlank)
						continue;

          for (int c = 0; c < cells.MaxDataColumn + 1; c++) {
            if (cells[r,c] == null)
              continue;

            if (r == GetRowIndexForFirstStudentItemization(template) && c == 1) // Number column
            {
              cells[r,c].PutValue(((s + 1) * per) + 1);
              continue;
            }

            if (!(cells[r, c].Type == CellValueType.IsString))
              continue;

            var value = cells[r,c].StringValue;

            const string pattern = @"Students\[(\d+)\]";
            var matches = Regex.Matches(value, pattern);
            if (matches.Count > 0)
            {
              var match = matches[0];
              var i = int.Parse(match.Groups[1].Value);

              value = Regex.Replace(value, pattern, $"Students[{(i + ((s + 1) * per))}]");
              cells[r,c].PutValue(value);
            }
          }
        }
      }
    }

    private string GenerateSchoolYear(string scope)
    {
      string year;

      if (scope.Contains(@"\d{4}-\d{4}")) {
        year = scope;
      }
      else {
        var components = scope.Split('.');
        if (new[] { 7, 8, 9, 10, 11, 12}.Contains(int.Parse(components[1]))) {
          year = $"{components[0] + 1}-{components[0]}";
        }
        else {
          year = $"{components[0]}-{components[0] + 1}";
        }
      }

      return year;
    } 

    private Report CreateInvoice(DateTime time, Template invoiceTemplate, CreateReport create)
    {
      // get reporter
      var reporter = _reporters.CreateInvoiceReporter(_context);

      // build config
      var config = new InvoiceReporter.Config
      {
        Scope = create.Invoice.Scope,
        InvoiceNumber = create.Name,
        SchoolYear = create.SchoolYear,
        AsOf = create.Invoice.AsOf,
        Prepared = time,
        ToSchoolDistrict = create.Invoice.ToSchoolDistrict,
        ToPDE = create.Invoice.ToPDE,
        SchoolDistrictAun = create.Invoice.SchoolDistrictAun,
      };

      // generate data
      var invoice = reporter.GenerateReport(config);

      // compose workbook
      var wb = new Workbook(new MemoryStream(invoiceTemplate.Content));
      InitializeWorkbookSheetPrinterMargins(wb);

      if (invoice.Students.Count > 0) {
        CloneInvoiceSheets(wb, invoice.Students.Count, invoiceTemplate);
      }
      else {
        wb.Worksheets.RemoveAt(1);
      }

      // generate xlsx
      var data = JsonConvert.SerializeObject(invoice);
      wb = _exporter.Export(wb, JsonConvert.DeserializeObject(data));

      // create report
      Report report;
      using (var xlsxms = new MemoryStream())
      using (var pdfms = new MemoryStream())
      {
        // wb.Write(ms);
        wb.Save(xlsxms, new XlsSaveOptions(SaveFormat.Xlsx));
        wb.CalculateFormula();
        wb.Save(pdfms, new XlsSaveOptions(SaveFormat.Pdf));

        report = new Report
        {
          Type = ReportType.Invoice,
          Name = create.Name,
          SchoolYear = create.Invoice.Scope != null && create.Invoice.Scope.Length > 0 ? GenerateSchoolYear(create.Invoice.Scope) : create.SchoolYear,
					Scope = create.Invoice.Scope,
          Approved = false,
          Created = time,
          Data = data,
          Xlsx = xlsxms.ToArray(),
          Pdf = pdfms.ToArray() 
        };
      }

      return report;
    }

    private class MissingTemplateException : Exception
    {
      public MissingTemplateException(int templateId) :
        base($"Could not find template with Id '{templateId}'.")
      { }
    }

    private Report CreateInvoice(DateTime time, CreateReport create)
    {
      // get template
      var invoiceTemplate = _templates.Get(create.TemplateId);
      if (invoiceTemplate == null)
        throw new MissingTemplateException(create.TemplateId);

      return CreateInvoice(time, invoiceTemplate, create);
    }

    private Report CreateBulkInvoice(DateTime time, CreateReport create, IList<Report> reports)
    {
      // get template
      var invoiceTemplate = _templates.Get(create.TemplateId);
      if (invoiceTemplate == null)
        throw new MissingTemplateException(create.TemplateId);


      return CreateBulkInvoice(time, reports, invoiceTemplate, create);
    }

    private Report CreateInvoice(CreateReport create) => CreateInvoice(DateTime.Now, create);
    private Report CreateBulkInvoice(CreateReport create, IList<Report> reports) => CreateBulkInvoice(DateTime.Now, create, reports);

    [HttpPost]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(typeof(ReportResponse), 200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    [ProducesResponseType(409)]
    [ProducesResponseType(424)]
    [SwaggerResponse(statusCode: 501, description: "Not Implemented")] // Swashbuckle sees this as "Server Error".
    public async Task<IActionResult> Create([FromBody]CreateReport create)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      Report report;
      try
      {
        if (create.ReportType == ReportType.Invoice.Value)
        {
          if (create.Invoice == null)
            return new BadRequestObjectResult(new ErrorsResponse("Cannot create invoice without 'invoice' config."));

          report = CreateInvoice(create);
        }
        else
        {
          return StatusCode(501);
        }
      }
      catch (MissingTemplateException e)
      {
        var result = new ObjectResult(new ErrorResponse(e.Message));
        result.StatusCode = 424;
        return result;
      }

      try
      {
        report = await Task.Run(() => _context.SaveChanges(() => _reports.Create(report)));
      }
      catch (DbUpdateException)
      {
        return StatusCode(409);
      }

      return new CreatedResult($"/api/reports/{report.Name}", new ReportResponse
      {
        Report = new ReportDto(report),
      });
    }

    public class CreateManyInvoiceReports
    {
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }
      [Required]
      public DateTime AsOf { get; set; }

      [Required]
      public DateTime ToSchoolDistrict { get; set; }

      [Required]
      public DateTime ToPDE { get; set; }
    }

    public class CreateManyReports
    {
			[EnumerationValidation(typeof(ReportType))]
			[Required]
			public string ReportType { get; set; }

			[Required]
			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }

			[Required]
			[Range(1, int.MaxValue)]
			public int TemplateId { get; set; }

			public CreateManyInvoiceReports Invoice { get; set; }
    }

    private IList<Report> CreateManyInvoices(CreateManyReports create)
    {
      var invoiceTemplate = _templates.Get(create.TemplateId);
      if (invoiceTemplate == null)
        throw new MissingTemplateException(create.TemplateId);

      var reports = new List<Report>();
      var now = DateTime.Now;
      var names = _districts.GetManyNames();
      var stamp = new Random(DateTime.Now.Millisecond).Next();

      foreach (var name in names)
      {
        var sd = _districts.GetByName(name);
        if (sd != null)
        {
          var month = create.Invoice.AsOf.ToString("MMMM");
          reports.Add(CreateInvoice(now, invoiceTemplate, new CreateReport
          {
            ReportType = create.ReportType,
            Name = $"{create.Invoice.Scope}_{sd.Name}_{stamp}",
            SchoolYear = create.SchoolYear,
            TemplateId = create.TemplateId,
            Invoice = new CreateInvoiceReport
            {
              Scope = create.Invoice.Scope,
              AsOf = create.Invoice.AsOf,
              ToSchoolDistrict = create.Invoice.ToSchoolDistrict,
              ToPDE = create.Invoice.ToPDE,
              SchoolDistrictAun = sd.Aun,
            }
          }));
        }
      }

      return reports;
    }

    private static readonly object _lock = new object();

    public delegate string HeaderMapper(string key);

    public static string MapStudentActivityHeaderKeyToValue(string key)
    {
      Dictionary<string, string> _labels = new Dictionary<string, string>
      {
        { "Number", "Number" },
        { "SchoolDistrict","School District" },
        { "StudentName","Student Name" },
        { "Address", "Address" },
        { "CityStateZip", "City, State Zip" },
        { "BirthDate", "Birth Date" },
        { "GradeLevel", "Grade Level" },
        { "FirstDateEducated", "First Date Educated" },
        { "LastDateEducated", "Last Date Educated" },
        { "SPED", "SPED" },
        { "CurrentIEP", "Current IEP" },
        { "PriorIEP", "Prior IEP" }
      };

      string value;
      if (_labels.TryGetValue(key, out value))
      {
        return value;
      }

      return null;
    }

    private DataTable BuildStudentActivityDataTable(IEnumerable<Report> invoices)
    {
      DataTable studentActivityDataTable = new DataTable();

      AddColumnsToStudentActivityDataTable(studentActivityDataTable);

      foreach (var invoice in invoices)
      {
        var students = JObject.Parse(invoice.Data)["Students"];
        var schoolDistrict = JObject.Parse(invoice.Data)["SchoolDistrict"];
        var studentsJSONStr = JsonConvert.SerializeObject(students);
        DataTable dt = GetDataTableFromJsonString(studentsJSONStr);

        // we only want the student data...
        AddRowsToStudentActivityDataTable(studentActivityDataTable, dt, (JObject)schoolDistrict);
      }

      return studentActivityDataTable;
    }

    private void AddColumnsToStudentActivityDataTable(DataTable dt)
    {
      dt.Columns.Add("Number", typeof(uint));
      dt.Columns.Add("SchoolDistrict", typeof(string));
      dt.Columns.Add("StudentName", typeof(string));
      dt.Columns.Add("Address", typeof(string));
      dt.Columns.Add("CityStateZip", typeof(string));
      dt.Columns.Add("BirthDate", typeof(DateTime));
      dt.Columns.Add("GradeLevel", typeof(string));
      dt.Columns.Add("FirstDateEducated", typeof(DateTime));
      dt.Columns.Add("LastDateEducated", typeof(DateTime));
      dt.Columns.Add("SPED", typeof(string));
      dt.Columns.Add("CurrentIEP", typeof(DateTime));
      dt.Columns.Add("PriorIEP", typeof(DateTime));
      dt.AcceptChanges();
    }

    private void AddRowsToStudentActivityDataTable(DataTable dtDest, DataTable dt, JObject schoolDistrict)
    {
      int i = 0;
      foreach (var row in dt.Rows)
      {
        AddRowToStudentActivityDataTable(dtDest, (DataRow)row, schoolDistrict, ++i);
      }
    }

    private void AddRowToStudentActivityDataTable(DataTable dtDest, DataRow row, JObject schoolDistrict, int index)
    {
      DataRow newRow = dtDest.NewRow();

      newRow["Number"] = index;
      newRow["SchoolDistrict"] = schoolDistrict["Name"];
      newRow["StudentName"] = row["LastName"] + ", " + row["FirstName"] + " " + row["MiddleInitial"];
      newRow["Address"] = row["Address1"];
      newRow["CityStateZip"] = row["Address2"];
      newRow["BirthDate"] = row["DateOfBirth"];
      newRow["GradeLevel"] = row["Grade"];
      newRow["FirstDateEducated"] = row["FirstDay"];
      newRow["LastDateEducated"] = row["LastDay"];
      newRow["SPED"] = row["IsSpecialEducation"].ToString() == "True" ? "Y" : "N";
      newRow["CurrentIEP"] = row["CurrentIep"];
      newRow["PriorIEP"] = row["FormerIep"];

      dtDest.Rows.Add(newRow);
    }

    private DataTable GetDataTableFromJsonString(string json)
    {
      return JsonConvert.DeserializeObject<DataTable>(json);
    }

    [HttpPost("many")]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(typeof(ReportsResponse), 200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    [ProducesResponseType(409)]
    [SwaggerResponse(statusCode: 423, description: "Locked")] // Swashbuckle sees this as "Client Error".
    [ProducesResponseType(424)]
    [SwaggerResponse(statusCode: 501, description: "Not Implemented")] // Swashbuckle sees this as "Server Error".
    public IActionResult CreateMany([FromBody]CreateManyReports create)
    {
      var acquired = false;
      try
      {
        Monitor.TryEnter(_lock, ref acquired);
        if (!acquired)
          return StatusCode(423);

        if (!ModelState.IsValid)
          return new BadRequestObjectResult(new ErrorsResponse(ModelState));

        IList<Report> reports = null;
        try
        {
          if (create.ReportType == ReportType.Invoice.Value)
          {
            if (create.Invoice == null)
              return new BadRequestObjectResult(new ErrorsResponse("Cannot create invoice without 'invoice' config."));

            reports = CreateManyInvoices(create);
          }
          else
          {
            return StatusCode(501);
          }
        }
        catch (MissingTemplateException e)
        {
          var result = new ObjectResult(new ErrorResponse(e.Message));
          result.StatusCode = 424;
          return result;
        }

        try
        {
          reports = _context.SaveChanges(() => _reports.CreateMany(reports));
        }
        catch (DbUpdateException)
        {
          return StatusCode(409);
        }

        return new CreatedResult($"/api/reports?type={create.ReportType}&scope={create.SchoolYear}&approved=false", new ReportsResponse
        {
          Reports = reports.OrderBy(r => r.Id).Select(r => new ReportDto(r)).ToList(),
        });
      }
      finally
      {
        if (acquired)
          Monitor.Exit(_lock);
      }
    }

    public struct ReportResponse
    {
      public ReportDto Report { get; set; }
    }

    public class GetInvoiceArgs
    {
			public string Name { get; set; }
      public string Format { get; set; }
    }

    [HttpGet("invoice/name")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX, ContentTypes.PDF)]
    [ProducesResponseType(404)]
    [ProducesResponseType(406)]
    public async Task<IActionResult> GetInvoice([FromQuery]GetInvoiceArgs args)
    {
      var report = await Task.Run(() => _reports.Get(args.Name));
      if (report == null)
        return NotFound();

      var accept = Request.Headers["Accept"];
      foreach (var v in accept) {
        if (!v.Contains(ContentTypes.XLSX) && !v.Contains(ContentTypes.PDF))
          return StatusCode(406);
      }

      var name = args.Name;
      var contentType = args.Format == "excel" ? ContentTypes.XLSX : ContentTypes.PDF;
      var data = contentType == ContentTypes.XLSX ? report.Xlsx : report.Pdf;

      using (var stream = new MemoryStream(data))
      {
        return new FileStreamResult(new MemoryStream(stream.ToArray()), contentType)
        {
          FileDownloadName = name
        };
      };
    }

    [HttpGet("{name}")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX, ContentTypes.PDF)]
    [ProducesResponseType(404)]
    [ProducesResponseType(406)]
    public async Task<IActionResult> Get(string name)
    {
      var report = await Task.Run(() => _reports.Get(name));
      if (report == null)
        return NotFound();

      var accept = Request.Headers["Accept"];

      foreach (var v in accept) {
        if (!v.Contains(ContentTypes.XLSX) && !v.Contains(ContentTypes.PDF))
          return StatusCode(406);
      }

      var stream = new MemoryStream(report.Xlsx);
      return new FileStreamResult(stream, ContentTypes.XLSX)
      {
        FileDownloadName = report.Name,
      };
    }

    public class GetActivityArgs
    {
			public string Name { get; set; }

      public string Format { get; set; }

      public string Type { get; set; }

      public string SchoolYear { get; set; }

      public bool? Approved { get; set; }
    }

    [HttpGet("activity/name")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX, ContentTypes.PDF)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetActivity([FromQuery]GetActivityArgs args)
    {
      var report = await Task.Run(() => _reports.Get(args.Name));
      if (report == null)
        return NotFound();

      var accept = Request.Headers["Accept"];
      foreach (var v in accept) {
        if (!v.Contains(ContentTypes.XLSX) && !v.Contains(ContentTypes.PDF))
          return StatusCode(406);
      }

      var name = args.Name + "_ACTIVITY";
      List<Report> reports = new List<Report>();
      reports.Add(report);
      var data = BuildStudentActivityDataTable(reports);
      Workbook wb = new Workbook();

      wb.Worksheets[0].Cells.ImportDataTable(data, true, 0, 0, true, false);
      Style style = wb.CreateStyle();
      style.Number = 14;
      StyleFlag styleFlag = new StyleFlag();
      styleFlag.NumberFormat = true;
      wb.Worksheets[0].Cells.Columns[5].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[7].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[8].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[10].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[11].ApplyStyle(style, styleFlag);

      var saveOpts = new XlsSaveOptions(args.Format == "excel" ? SaveFormat.Xlsx : SaveFormat.Pdf);

      using (var stream = new MemoryStream())
      {
        wb.Save(stream, saveOpts);
        var contentType = args.Format == "excel" ? ContentTypes.XLSX : ContentTypes.PDF;
        return new FileStreamResult(new MemoryStream(stream.ToArray()), contentType)
        {
          FileDownloadName = name
        };
      };
    }

    [HttpGet("activity")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX, ContentTypes.PDF)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetBulkActivity([FromQuery]GetActivityArgs args)
    {
      var accept = Request.Headers["Accept"];

      foreach (var v in accept) {
        if (!v.Contains(ContentTypes.XLSX) && !v.Contains(ContentTypes.PDF))
          return StatusCode(406);
      }

      var reports = await Task.Run(() => _reports.GetMany(
        type: args.Type == null ? null : ReportType.FromString(args.Type),
        year: args.SchoolYear,
        approved: args.Approved
      ));

      if (reports == null)
        return NotFound();

      var data = BuildStudentActivityDataTable(reports);
      string name = args.SchoolYear+ "Student Activity";
      Workbook wb = new Workbook();

      wb.Worksheets[0].Cells.ImportDataTable(data, true, 0, 0, true, false);
      Style style = wb.CreateStyle();
      style.Number = 14;
      StyleFlag styleFlag = new StyleFlag();
      styleFlag.NumberFormat = true;
      wb.Worksheets[0].Cells.Columns[5].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[7].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[8].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[10].ApplyStyle(style, styleFlag);
      wb.Worksheets[0].Cells.Columns[11].ApplyStyle(style, styleFlag);

      var saveOpts = new XlsSaveOptions(args.Format == "excel" ? SaveFormat.Xlsx : SaveFormat.Pdf);

      using (var stream = new MemoryStream())
      {
        wb.Save(stream, saveOpts);
        var contentType = args.Format == "excel" ? ContentTypes.XLSX : ContentTypes.PDF;
        return new FileStreamResult(new MemoryStream(stream.ToArray()), ContentTypes.XLSX)
        {
          FileDownloadName = name
        };
      };
    }

    public class GetManyArgs
    {
			public string Name { get; set; }

			[EnumerationValidation(typeof(ReportType))]
			public string Type { get; set; }

			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }

			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

			public bool? Approved { get; set; }
    }

    public struct ReportsResponse
    {
      public IList<ReportDto> Reports { get; set; }
    }

    [HttpGet("zip")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.ZIP)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetZip([FromQuery]GetManyArgs args)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      var reports = await Task.Run(() => _reports.GetMany(
        name: args.Name,
        type: args.Type == null ? null : ReportType.FromString(args.Type),
        year: args.SchoolYear,
				scope: args.Scope,
        approved: args.Approved
      ));
      if (reports == null)
        return NoContent();

      var content = false; // avoids using .Count(), which executes the IEnumerable
      var zipStream = new MemoryStream();
      using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
      {
        foreach (var report in reports)
        {
          content = true;
          var entry = archive.CreateEntry($"{report.Name}.xlsx");
          using (var entryStream = entry.Open())
            entryStream.Write(report.Xlsx, 0, report.Xlsx.Length);
        }
      }
      zipStream.Position = 0;

      if (!content)
        return NoContent();

      var name = new StringBuilder("Reports");
      if (!string.IsNullOrWhiteSpace(args.SchoolYear))
        name.Append($"-{args.SchoolYear}");

      if (!string.IsNullOrEmpty(args.Type))
        name.Append($"-{args.Type}");

      if (!string.IsNullOrWhiteSpace(args.Name))
        name.Append($"-{args.Name}");

      if (args.Approved.HasValue)
        name.Append(args.Approved.Value ? "-Approved" : "-Pending");

      return new FileStreamResult(zipStream, ContentTypes.ZIP)
      {
        FileDownloadName = name.ToString(),
      };
    }

    private void InitializeWorkbookSheetPrinterMargins(Workbook wb)
    {
      for (int i = 0; i < wb.Worksheets.Count; i++) {
        var sheet = wb.Worksheets[i];
        if (sheet != null) {
          // make certain the printer margins for each sheet are zeroed out, lest
          // we have ugly issues when printing them out.
          sheet.PageSetup.HeaderMargin = 0.0;
          sheet.PageSetup.FooterMargin = 0.0;
          sheet.PageSetup.TopMargin = 0.0;
          sheet.PageSetup.TopMargin = 0.0;
          sheet.PageSetup.LeftMargin = 0.0;
          sheet.PageSetup.RightMargin = 0.0;
          sheet.PageSetup.HeaderMargin = 0.0;
          sheet.PageSetup.FooterMargin = 0.0;
        } 
      }
    }

    private Report CreateBulkInvoice(DateTime time, IList<Report> reports, Template invoiceTemplate, CreateReport create)
    {
      var invoices = reports.Select(r => JsonConvert.DeserializeObject<Invoice>(r.Data)).OrderBy(i => i.SchoolDistrict.Name).ToList();

      // compose workbook
      var wb = new Workbook(new MemoryStream(invoiceTemplate.Content));
      InitializeWorkbookSheetPrinterMargins(wb);
      Console.WriteLine($"ReportsController.CreateBulkInvoice():  number of invoices is {invoices.Count}.");
      for (int i = 0; i < invoices.Count; i++) {
        var invoice = invoices[i];

        if (i > 0) {
          CloneInvoiceSummarySheet(wb, i);
        }

        if (invoice.Students.Count > 0)
          CloneStudentItemizationSheets(wb, invoice.Students.Count, i, invoiceTemplate);
      }

      if (invoices[0].Students.Count == 0) {
        wb.Worksheets.RemoveAt(1);
      }
      // generate xlsx
      
      var data = new {
        SchoolYear = create.SchoolYear,
        AsOf = create.BulkInvoice.AsOf,
        Prepared = time,
        ToSchoolDistrict = create.BulkInvoice.ToSchoolDistrict,
        ToPDE = create.BulkInvoice.ToPDE,
        Districts = invoices.Select(i => new {
          Number = i.Number,
          SchoolDistrict = i.SchoolDistrict,
          Students = i.Students,
          RegularEnrollments = i.RegularEnrollments,
          SpecialEnrollments = i.SpecialEnrollments,
          Transactions = i.Transactions
         })
      };

      var json = JsonConvert.SerializeObject(data);
      wb = _exporter.Export(wb, JsonConvert.DeserializeObject(json));

      // create report
      Report report;
      using (var ms = new MemoryStream())
      using (var pdfms = new MemoryStream())
      {
        // wb.Write(ms);
        wb.Save(ms, new XlsSaveOptions(SaveFormat.Xlsx));
        wb.CalculateFormula();
        wb.Save(pdfms, new XlsSaveOptions(SaveFormat.Pdf));

        report = new Report
        {
          Type = ReportType.BulkInvoice,
					SchoolYear = create.SchoolYear,
					Scope = create.BulkInvoice.Scope,
          Name = create.Name,
          Approved = true,
          Created = time,
          Data = json,
          Xlsx = ms.ToArray(),
          Pdf = pdfms.ToArray()
        };
      }

      return report;
    }

    [HttpPost("bulk")]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(typeof(ReportResponse), 200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    [ProducesResponseType(409)]
    [ProducesResponseType(424)]
    [SwaggerResponse(statusCode: 501, description: "Not Implemented")] // Swashbuckle sees this as "Server Error".
    public async Task<IActionResult> CreateBulk([FromBody]CreateReport create)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      Report report;

      try
      {
        if (create.ReportType == ReportType.BulkInvoice.Value)
        {
          if (create.BulkInvoice == null)
            return new BadRequestObjectResult(new ErrorsResponse("Cannot create invoice without 'bulk invoice' config."));

          var reports = await Task.Run(() => _reports.GetMany(
            type: ReportType.FromString("Invoice"),
            year: create.SchoolYear,
            approved: create.BulkInvoice.Approved
          ));

          report = CreateBulkInvoice(create, reports.ToList());
        }
        else
        {
          return StatusCode(501);
        }
      }
      catch (MissingTemplateException e)
      {
        var result = new ObjectResult(new ErrorResponse(e.Message));
        result.StatusCode = 424;
        return result;
      }

      try
      {
        report = await Task.Run(() => _context.SaveChanges(() => _reports.Create(report)));
      }
      catch (DbUpdateException)
      {
        return StatusCode(409);
      }

      return new CreatedResult($"/api/reports/{report.Name}", new ReportResponse
      {
        Report = new ReportDto(report),
      });
    }

    [HttpGet]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(typeof(ReportsResponse), 200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetManyMetadata([FromQuery]GetManyArgs args)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      var reports = await Task.Run(() => _reports.GetManyMetadata(
        name: args.Name,
        type: args.Type == null ? null : ReportType.FromString(args.Type),
        year: args.SchoolYear,
				scope: args.Scope,
        approved: args.Approved
      ));
      if (reports == null)
        reports = new List<ReportMetadata>();

      return new ObjectResult(new ReportsResponse
      {
        Reports = reports.Select(r => new ReportDto(r)).ToList(),
      });
    }

    [HttpPost("{name}/approve")]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Approve(string name)
    {
      try
      {
        await Task.Run(() => _context.SaveChanges(() => _reports.Approve(name)));
      }
      catch (NotFoundException)
      {
        return NotFound();
      }

      return Ok();
    }

    [HttpPost("{name}/reject")]
    [Authorize(Policy = "PAY+")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Reject(string name)
    {
      await Task.Run(() => _context.SaveChanges(() => _reports.Reject(name)));
      return Ok();
    }
  }
}
