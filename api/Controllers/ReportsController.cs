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

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Swashbuckle.AspNetCore.SwaggerGen;

using api.Common;
using api.Common.Utils;
using api.Dtos;
using models;
using models.Reporters;
using models.Reporters.Exporters;

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
			public DateTime AsOf { get; set; }

			[Required]
			public DateTime ToSchoolDistrict { get; set; }

			[Required]
			public DateTime ToPDE { get; set; }

			[Required]
			[Range(100000000, 999999999)]
			public int SchoolDistrictAun { get; set; }
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
		}

		private void CloneInvoiceSheets(XSSFWorkbook wb, int count)
		{
			const int per = 8;

			var numSheets = (int)count / per + (count % per == 0 ? 0 : 1);
			for (var s = 0; s < numSheets - 1; s++)
			{
				wb.CloneSheet(1);

				var sheet = wb.GetSheetAt(s + 2);
				for (var r = sheet.FirstRowNum; r < sheet.LastRowNum; r++)
				{
					var row = sheet.GetRow(r);
					if (row.Cells.All(c => c.CellType == CellType.Blank))
						continue;

					for (var c = row.FirstCellNum; c < row.LastCellNum; c++)
					{
						var cell = row.GetCell(c);
						if (cell == null)
							continue;

						if (r == 13 && c == 1) // Number column
						{
							cell.SetCellValue(((s + 1) * per) + 1);
							continue;
						}

						if (!(cell.CellType == CellType.String))
							continue;

						var value = cell.StringCellValue;
						if (!value.Contains("${Students["))
							continue;

						const string pattern = @"\[(\d+)\]";
						var matches = Regex.Matches(value, pattern);
						if (matches.Count > 0)
						{
							var match = matches[0];
							var i = int.Parse(match.Groups[1].Value);

							value = Regex.Replace(value, pattern, $"[{(i + ((s + 1) * per))}]");
							cell.SetCellValue(value);
						}
					}
				}
			}
		}

		private Report CreateInvoice(DateTime time, Template invoiceTemplate, CreateReport create)
		{
			// get reporter
			var reporter = _reporters.CreateInvoiceReporter(_context);

			// build config
			var config = new InvoiceReporter.Config
			{
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
			var wb = new XSSFWorkbook(new MemoryStream(invoiceTemplate.Content));

			if (invoice.Students.Count > 0)
				CloneInvoiceSheets(wb, invoice.Students.Count);
			else
				wb.RemoveSheetAt(1);

			// generate xlsx
			var data = JsonConvert.SerializeObject(invoice);
			wb = _exporter.Export(wb, JsonConvert.DeserializeObject(data));

			// create report
			Report report;
			using (var ms = new MemoryStream())
			{
				wb.Write(ms);

				report = new Report
				{
					Type = ReportType.Invoice,
					SchoolYear = create.SchoolYear,
					Name = create.Name,
					Approved = false,
					Created = time,
					Data = data,
					Xlsx = ms.ToArray(),
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

		private Report CreateInvoice(CreateReport create) => CreateInvoice(DateTime.Now, create);

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
      foreach (var name in names) {
        var sd = _districts.GetByName(name);
        if (sd != null) {
          var month = create.Invoice.AsOf.ToString("MMMM");
          reports.Add(CreateInvoice(now, invoiceTemplate, new CreateReport
          {
            ReportType = create.ReportType,
            Name = $"{create.SchoolYear}_{sd.Name}_{month}-{create.Invoice.AsOf.Year}",
            SchoolYear = create.SchoolYear,
            TemplateId = create.TemplateId,

            Invoice = new CreateInvoiceReport
            {
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

		private void MergeInvoiceActivityData(XSSFSheet sheet, XSSFWorkbook wb, int idx)
		{
      if (wb != null) {
        if (sheet != null) {
          XSSFSheet destSheet = (XSSFSheet)wb.CreateSheet(sheet.SheetName);
          NPOIHelper.CopySheets(destSheet, sheet);
        }
      }
		}

		private bool IsActivityWorksheet(XSSFSheet sheet)
		{
			if (sheet != null) {
				const string pattern = @"^Individual Student Inform.*";
				var matches = Regex.Matches(sheet.SheetName, pattern);
				if (matches.Count > 0)
				{
					return true;
				}
			}

			return false;
		}

		private DataTable BuildStudentActivityDataTable(IEnumerable<Report> invoices)
		{
      DataTable studentActivityDataTable = new DataTable();

      AddColumnsToStudentActivityDataTable(studentActivityDataTable);
      foreach (var invoice in invoices) {
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
      dt.Columns.Add("BirthDate", typeof(string));
      dt.Columns.Add("GradeLevel", typeof(string));
      dt.Columns.Add("FirstDateEducated", typeof(string));
      dt.Columns.Add("LastDateEducated", typeof(string));
      dt.Columns.Add("SPED", typeof(string));
      dt.Columns.Add("CurrentIEP", typeof(string));
      dt.Columns.Add("PriorIEP", typeof(string));
      dt.AcceptChanges();
    }

    private void AddRowsToStudentActivityDataTable(DataTable dtDest, DataTable dt, JObject schoolDistrict)
    {
      int i = 0;
      _logger.LogInformation($"ReportsController.AddRowsToStudentActivityDataTable():  schoolDistrict is " + schoolDistrict["Name"]);
      foreach (var row in dt.Rows) {
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
      newRow["SPED"] = row["IsSpecialEducation"];
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

				return new CreatedResult($"/api/reports?type={create.ReportType}&schoolYear={create.SchoolYear}&approved=false", new ReportsResponse
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

		[HttpGet("{name}")]
		[Authorize(Policy = "PAY+")]
		[Produces(ContentTypes.XLSX)]
		[ProducesResponseType(404)]
		[ProducesResponseType(406)]
		public async Task<IActionResult> Get(string name)
		{
			var report = await Task.Run(() => _reports.Get(name));
			if (report == null)
				return NotFound();

			var accept = Request.Headers["Accept"];
			Console.WriteLine($"{accept}");

			if (accept != ContentTypes.XLSX)
				return StatusCode(406);

			var stream = new MemoryStream(report.Xlsx);
			return new FileStreamResult(stream, ContentTypes.XLSX)
			{
				FileDownloadName = report.Name,
			};
		}

    public class GetActivityArgs
    {
      public string Name { get; set; }

      [EnumerationValidation(typeof(ReportType))]
      public string Type { get; set; }

      [RegularExpression(@"^\d{4}\-\d{4}$")]
      public string SchoolYear { get; set; }

      public bool? Approved { get; set; }
    }

    [HttpGet("activity/name/{name}")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetActivity(string name)
    {
      var report = await Task.Run(() => _reports.Get(name));
      if (report == null)
        return NotFound();

      var accept = Request.Headers["Accept"];
      Console.WriteLine($"{accept}");

      if (accept != ContentTypes.XLSX)
        return StatusCode(406);

      List<Report> reports = new List<Report>();
      reports.Add(report);
      var data = BuildStudentActivityDataTable(reports);
      XSSFWorkbook wb = NPOIHelper.BuildExcelWorkbookFromDataTable(data, name);
      var stream = new MemoryStream();
      wb.Write(stream);
      return new FileStreamResult(stream, ContentTypes.XLSX)
      {
        FileDownloadName = report.Name,
      };
    }

    [HttpGet("activity")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX)]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> GetBulkActivity([FromQuery]GetActivityArgs args)
    {
      var accept = Request.Headers["Accept"];
      Console.WriteLine($"{accept}");

      if (accept != ContentTypes.XLSX)
        return StatusCode(406);

      var reports = await Task.Run(() => _reports.GetMany(
				type: args.Type == null ? null : ReportType.FromString(args.Type),
				year: args.SchoolYear,
				approved: args.Approved
      ));

      if (reports == null)
        return NotFound();

      var data = BuildStudentActivityDataTable(reports);
      string name = args.SchoolYear + "Student Activity";
      XSSFWorkbook wb = NPOIHelper.BuildExcelWorkbookFromDataTable(data, name);


      using (var stream = new MemoryStream())
      {
        wb.Write(stream);

        _logger.LogInformation($"ReportsController.GetBulkActivity():  workbook is {stream.ToArray()}.");
        return new FileStreamResult(new MemoryStream(stream.ToArray()), ContentTypes.XLSX)
        {
          FileDownloadName = name
        };
      }
    }

    public class GetManyArgs
		{
			public string Name { get; set; }

			[EnumerationValidation(typeof(ReportType))]
			public string Type { get; set; }

			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }

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

    public class GetBulkArgs
		{
			[EnumerationValidation(typeof(ReportType))]
			public string Type { get; set; }

			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }

			public bool? Approved { get; set; }
		}

    private byte[] CreateMergedInvoicesWorkbook(IEnumerable<Report> reports)
    {
      XSSFWorkbook wb = new XSSFWorkbook();
      wb.CreateSheet();
      foreach (var report in reports)
      {
        // all data for the bulk invoice spreadsheet are on a single worksheet
        MemoryStream ms = new MemoryStream(report.Xlsx);
        _logger.LogInformation($"ReportsController.CreateMergedInvoicesWorkbook():  report {report.Name} length is {ms.Length}.");
        XSSFWorkbook wb1 = new XSSFWorkbook(new MemoryStream(report.Xlsx));
        for (int i = 0; i < wb1.NumberOfSheets; i++) {
          if (IsActivityWorksheet((XSSFSheet)wb1.GetSheetAt(i))) {
            NPOIHelper.MergeSheets((XSSFSheet)wb.GetSheetAt(0), (XSSFSheet)wb1.GetSheetAt(i));
          }
        }
      }

      var data = new MemoryStream();
      wb.Write(data);

      return data.ToArray();
    }

    [HttpGet("bulk")]
    [Authorize(Policy = "PAY+")]
    [Produces(ContentTypes.XLSX)]
		[ProducesResponseType(204)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		public async Task<IActionResult> GetBulkInvoice([FromQuery]GetBulkArgs args)
    {
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      var accept = Request.Headers["Accept"];
			Console.WriteLine($"{accept}");

			if (accept != ContentTypes.XLSX)
				return StatusCode(406);

			var reports = await Task.Run(() => _reports.GetMany(
				type: args.Type == null ? null : ReportType.FromString(args.Type),
				year: args.SchoolYear,
				approved: args.Approved
			));

			if (reports == null)
				return NoContent();

      byte[] invoice = CreateMergedInvoicesWorkbook(reports);
      _logger.LogInformation($"ReportsController.GetBulkInvoice():  length of merged invoices is {invoice.Length}.");
      var stream = new MemoryStream(invoice);
      
			return new FileStreamResult(stream, ContentTypes.XLSX)
			{
				FileDownloadName = $"{args.SchoolYear}_{args.Type}",
			};
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
