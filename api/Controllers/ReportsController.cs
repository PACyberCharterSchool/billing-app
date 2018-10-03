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

using Aspose.Cells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

using api.Common;
using api.Common.Utils;
using api.Dtos;
using models;
using models.Reporters;
using models.Reporters.Exporters;
using System.Drawing;

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
			public DateTime ToSchoolDistrict { get; set; }

			[Required]
			public DateTime ToPDE { get; set; }

			[Required]
			[Range(100000000, 999999999)]
			public int SchoolDistrictAun { get; set; }

			public bool TotalsOnly { get; set; }
		}

		public class CreateBulkInvoiceReport
		{
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

			[Required]
			public DateTime ToSchoolDistrict { get; set; }

			[Required]
			public DateTime ToPDE { get; set; }

			public IList<int> Auns { get; set; }

			[JsonConverter(typeof(SchoolDistrictPaymentTypeJsonConverter))]
			public SchoolDistrictPaymentType PaymentType { get; set; }

			public bool TotalsOnly { get; set; }
		}

		public class CreateStudentInformationReport
		{
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

			[Required]
			public DateTime AsOf { get; set; }

			[Required]
			[Range(100000000, 999999999)]
			public int SchoolDistrictAun { get; set; }
		}

		public class CreateBulkStudentInformationReport
		{
			[Required]
			[RegularExpression(@"^\d{4}(?:\-\d{4}|\.\d{2})$")]
			public string Scope { get; set; }

			[Required]
			public DateTime AsOf { get; set; }

			public IList<int> Auns { get; set; }
		}

		public class CreateAccountsReceivableAsOfReport
		{
			[Required]
			public DateTime AsOf { get; set; }

			public IList<int> Auns { get; set; }
		}

		public class CreateCsiuReport
		{
			[Required]
			public DateTime AsOf { get; set; }

			public IList<int> Auns { get; set; }
		}

		public class CreateUniPayInvoiceSummaryReport
		{
			[Required]
			public DateTime AsOf { get; set; }
			// public IList<int> Auns { get; set; }
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

			public CreateInvoiceReport Invoice { get; set; }
			public CreateBulkInvoiceReport BulkInvoice { get; set; }
			public CreateStudentInformationReport StudentInformation { get; set; }
			public CreateBulkStudentInformationReport BulkStudentInformation { get; set; }
			public CreateAccountsReceivableAsOfReport AccountsReceivableAsOf { get; set; }
			public CreateCsiuReport Csiu { get; set; }

			public CreateUniPayInvoiceSummaryReport UniPayInvoiceSummary { get; set; }
		}

		private void StyleInvoiceWorksheet(Worksheet sheet)
		{
			sheet.PageSetup.SetFooter(1, "&P");
			sheet.PageSetup.HeaderMargin = 0.0;
			sheet.PageSetup.FooterMargin = 0.0;
			sheet.PageSetup.TopMargin = 0.0;
			sheet.PageSetup.LeftMargin = 0.0;
			sheet.PageSetup.RightMargin = 0.0;
		}

		private void CloneInvoiceSummarySheet(Workbook src, Workbook wb, int districtIndex, string districtName)
		{
			if (districtIndex > 0)
				wb.Worksheets.Add();

			var sheet = wb.Worksheets.Last();
			sheet.Copy(src.Worksheets[0]);
			sheet.Name = $"{districtName.Substring(0, Math.Min(districtName.Length, 17))} Summary Info";
			StyleInvoiceWorksheet(sheet);

			// all subsequent pages will be numbered starting from here
			sheet.PageSetup.FirstPageNumber = 1;

			var cells = sheet.Cells;
			for (int r = 0; r < cells.MaxDataRow + 1; r++)
				for (int c = 0; c < cells.MaxDataColumn + 1; c++)
				{
					var cell = cells[r, c];
					if (!(cell.Type == CellValueType.IsString))
						continue;

					var value = cell.StringValue;
					const string pattern = @"Districts\[(\d+)\]";
					var matches = Regex.Matches(value, pattern);
					if (matches.Count > 0)
					{
						var match = matches[0];
						var i = int.Parse(match.Groups[1].Value);
						cell.PutValue(Regex.Replace(value, pattern, $"Districts[{districtIndex}]"));
					}
				}
		}

		private void CloneStudentItemizationSheets(
			Workbook src,
			Workbook wb,
			int count,
			int districtIndex,
			string districtName,
			Template template)
		{
			const int entriesPerPage = 8;
			var remainder = count % entriesPerPage;
			var numSheets = count / entriesPerPage + (remainder == 0 ? 0 : 1);
			for (var s = 0; s < numSheets; s++)
			{
				wb.Worksheets.Add();
				var sheet = wb.Worksheets.Last();
				sheet.Copy(src.Worksheets[1]);
				sheet.Name = $"{districtName.Substring(0, Math.Min(districtName.Length, 15))} St. Info({s + 1})";
				StyleInvoiceWorksheet(sheet);

				var cells = sheet.Cells;
				for (int r = 0; r < cells.MaxDataRow + 1; r++)
					for (int c = 0; c < cells.MaxDataColumn + 1; c++)
					{
						var cell = cells[r, c];
						if (cell.Value == null)
							continue;

						if (r == GetRowIndexForFirstStudentItemization(template) && c == 1) // Number column
						{
							cell.PutValue((s * entriesPerPage) + 1);
							continue;
						}

						if (!(cell.Type == CellValueType.IsString))
							continue;

						{
							const string pattern = @"Students\[(\d+)\]";
							var matches = Regex.Matches(cell.StringValue, pattern);
							if (matches.Count > 0)
							{
								var match = matches[0];
								var i = int.Parse(match.Groups[1].Value);
								cell.PutValue(Regex.Replace(cell.StringValue, pattern, $"Students[{((s * entriesPerPage) + i)}]"));
							}
						}

						{
							const string pattern = @"Districts\[(\d+)\]";
							var matches = Regex.Matches(cell.StringValue, pattern);
							if (matches.Count > 0)
							{
								var match = matches[0];
								var i = int.Parse(match.Groups[1].Value);
								cell.PutValue(Regex.Replace(cell.StringValue, pattern, $"Districts[{districtIndex}]"));
							}
						}
					}
			}

			if (remainder == 0)
				return;

			const int rowsPerEntry = 4;
			var remove = entriesPerPage - remainder;
			var last = wb.Worksheets.Last();
			for (var i = remove * rowsPerEntry; i > 0; i--)
				last.Cells.DeleteRow(last.Cells.MaxDataRow);
		}

		private int GetRowIndexForFirstStudentItemization(Template template)
		{
			// What does this do you ask? Well, since we can't anticipate what the Pennsylvania
			// Department of Education is ever going to do with the invoice templates they provide
			// for use to cyber charter schools, we have to build logic in to find the cell that
			// has the index value for the first student in the student itemization.  The remainder
			// of the index values are calculated from the first index cell.
			// TODO(Erik): find programmatically by searching for cell with value == 1?
			if (template.SchoolYear.StartsWith("2018") && template.SchoolYear.EndsWith("2019"))
				return 12;

			return 13;
		}

		private Report CreateBulkInvoice(DateTime time, Template invoiceTemplate, CreateReport create)
		{
			var reporter = _reporters.CreateBulkInvoiceReporter(_context);
			var invoice = reporter.GenerateReport(new BulkInvoiceReporter.Config
			{
				SchoolYear = create.SchoolYear,
				Scope = create.BulkInvoice.Scope,
				Prepared = time,
				ToSchoolDistrict = create.BulkInvoice.ToSchoolDistrict,
				ToPDE = create.BulkInvoice.ToPDE,
				Auns = create.BulkInvoice.Auns,
				PaymentType = create.BulkInvoice.PaymentType,
			});

			// compose workbook
			var source = new Workbook(new MemoryStream(invoiceTemplate.Content));
			var wb = new Workbook();

			var districts = invoice.Districts.ToList();
			var summaryPages = new List<int>();
			for (int i = 0; i < districts.Count; i++)
			{
				var district = districts[i];
				CloneInvoiceSummarySheet(source, wb, i, district.SchoolDistrict.Name);

				summaryPages.Add(wb.Worksheets.Last().Index);

				if (create.BulkInvoice.TotalsOnly)
					continue;

				var studentCount = district.Students.Count();
				if (studentCount > 0)
					CloneStudentItemizationSheets(source, wb, studentCount, i, district.SchoolDistrict.Name, invoiceTemplate);
			}

			if (create.BulkInvoice.TotalsOnly)
				wb.Worksheets.RemoveAt(1);

			// generate xlsx
			var json = JsonConvert.SerializeObject(invoice);
			wb = _exporter.Export(wb, JsonConvert.DeserializeObject(json));

			// do this after data is actually in cells
			foreach (var p in summaryPages)
			{
				var sheet = wb.Worksheets[p];

				for (var r = 18; r <= 29; r++)
				{
					if (!sheet.Cells[r, 6].StringValue.Contains('\n'))
						continue;

					for (var c = 6; c <= 7; c++)
					{
						var style = sheet.Cells[r, c].GetStyle();
						style.IsTextWrapped = true;
						sheet.Cells[r, c].SetStyle(style);
					}

					var newlines = sheet.Cells[r, 6].StringValue.Count(s => s == '\n');
					sheet.Cells.SetRowHeight(r, sheet.Cells.GetRowHeight(r) * (newlines + 1));
				}
			}

			// create report
			Report report;
			using (var ms = new MemoryStream())
			using (var pdfms = new MemoryStream())
			{
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

		private Report CreateBulkInvoice(DateTime time, CreateReport create)
		{
			// get template
			var invoiceTemplate = _templates.Get(ReportType.BulkInvoice, create.SchoolYear);
			if (invoiceTemplate == null)
				throw new MissingTemplateException(ReportType.BulkInvoice, create.SchoolYear);


			return CreateBulkInvoice(time, invoiceTemplate, create);
		}

		private Report CreateBulkInvoice(CreateReport create) => CreateBulkInvoice(DateTime.Now, create);

		private Report CreateInvoice(DateTime time, Template template, CreateReport create)
		{
			var report = CreateBulkInvoice(time, template, new CreateReport
			{
				ReportType = create.ReportType,
				Name = create.Name,
				SchoolYear = create.SchoolYear,

				BulkInvoice = new CreateBulkInvoiceReport
				{
					Scope = create.Invoice.Scope,
					ToSchoolDistrict = create.Invoice.ToSchoolDistrict,
					ToPDE = create.Invoice.ToPDE,
					Auns = new[] { create.Invoice.SchoolDistrictAun },
					TotalsOnly = create.Invoice.TotalsOnly,
				},
			});

			report.Type = ReportType.Invoice;
			return report;
		}

		private Report CreateInvoice(DateTime time, CreateReport create)
		{
			var template = _templates.Get(ReportType.BulkInvoice, create.SchoolYear);
			if (template == null)
				throw new MissingTemplateException(ReportType.BulkInvoice, create.SchoolYear);

			return CreateInvoice(time, template, create);
		}

		private Report CreateInvoice(CreateReport create) => CreateInvoice(DateTime.Now, create);

		private static IList<string> studentInformationColumns = new List<string>
		{
			"Number",
			"SchoolDistrict",
			"StudentName",
			"Address",
			"CityStateZip",
			"BirthDate",
			"GradeLevel",
			"FirstDateEducated",
			"LastDateEducated",
			"SPED",
			"CurrentIEP",
			"PriorIEP",
		};

		private Report CreateBulkStudentInformation(CreateReport create)
		{
			var reporter = _reporters.CreateBulkStudentInformationReporter(_context);
			var result = reporter.GenerateReport(new BulkStudentInformationReporter.Config
			{
				Scope = create.BulkStudentInformation.Scope,
				AsOf = create.BulkStudentInformation.AsOf,
				Auns = create.BulkStudentInformation.Auns,
			});

			var wb = new Workbook();
			var ws = wb.Worksheets[0];

			var style = wb.CreateStyle();
			style.Number = 14;
			var styleFlag = new StyleFlag();
			styleFlag.NumberFormat = true;

			// column headers
			for (var i = 0; i < studentInformationColumns.Count; i++)
			{
				var column = studentInformationColumns[i];
				ws.Cells[0, i].PutValue(column);

				if (new[] { "BirthDate", "FirstDateEducated", "LastDateEducated", "CurrentIEP", "PriorIEP" }.Contains(column))
					ws.Cells.Columns[i].ApplyStyle(style, styleFlag);
			}

			var districtGroups = result.Students.GroupBy(s => s.SchoolDistrictName);
			var r = 1;
			foreach (var group in districtGroups)
			{
				var students = group.ToList();
				for (var i = 0; i < students.Count; i++)
				{
					var student = students[i];

					var c = 0;
					var cells = ws.Cells;
					cells[r, c++].PutValue(i + 1);
					cells[r, c++].PutValue(student.SchoolDistrictName);
					cells[r, c++].PutValue(student.FullName);
					cells[r, c++].PutValue(student.Address1);
					cells[r, c++].PutValue(student.CityStateZipCode);
					cells[r, c++].PutValue(student.DateOfBirth);
					cells[r, c++].PutValue(student.Grade);
					cells[r, c++].PutValue(student.FirstDay);
					cells[r, c++].PutValue(student.LastDay);
					cells[r, c++].PutValue(student.IsSpecialEducation ? "Y" : "N");
					cells[r, c++].PutValue(student.CurrentIep);
					cells[r, c++].PutValue(student.FormerIep);

					r++;
				}
			}

			Report report;
			using (var xlsxStream = new MemoryStream())
			using (var pdfStream = new MemoryStream())
			{
				wb.CalculateFormula();
				wb.Save(xlsxStream, SaveFormat.Xlsx);
				wb.Save(pdfStream, SaveFormat.Pdf);

				report = new Report
				{
					Type = ReportType.BulkStudentInformation,
					SchoolYear = create.SchoolYear,
					Name = create.Name,
					Approved = true,
					Created = DateTime.Now,
					Scope = create.BulkStudentInformation.Scope,
					Data = JsonConvert.SerializeObject(result),
					Xlsx = xlsxStream.ToArray(),
					Pdf = pdfStream.ToArray(),
				};
			}

			return report;
		}

		private Report CreateStudentInformation(CreateReport create)
		{
			var report = CreateBulkStudentInformation(new CreateReport
			{
				ReportType = create.ReportType,
				Name = create.Name,
				SchoolYear = create.SchoolYear,

				BulkStudentInformation = new CreateBulkStudentInformationReport
				{
					Scope = create.StudentInformation.Scope,
					AsOf = create.StudentInformation.AsOf,
					Auns = new[] { create.StudentInformation.SchoolDistrictAun },
				},
			});

			report.Type = ReportType.StudentInformation;
			return report;
		}

		private Report CreateAccountsReceivableAsOf(CreateReport create)
		{
			var reporter = _reporters.CreateAccountsReceivableAsOfReporter(_context);
			var result = reporter.GenerateReport(new AccountsReceivableAsOfReporter.Config
			{
				SchoolYear = create.SchoolYear,
				AsOf = create.AccountsReceivableAsOf.AsOf,
				Auns = create.AccountsReceivableAsOf.Auns,
			});

			var wb = new Workbook();
			var ws = wb.Worksheets[0];

			ws.PageSetup.SetFooter(1, "Page &P of &N");
			ws.PageSetup.SetFooter(2, result.AsOf.ToString("M/dd/yyyy"));
			ws.PageSetup.HeaderMargin = 0.0;
			ws.PageSetup.FooterMargin = 0.0;
			ws.PageSetup.TopMargin = 0.0;
			ws.PageSetup.LeftMargin = 0.0;
			ws.PageSetup.RightMargin = 0.0;

			var columnHeaders = new[] { "District", "Total Due", "Refunds", "Total Paid", "Net Due", "Payment Type" };

			var headerStyle = new CellsFactory().CreateStyle();
			headerStyle.HorizontalAlignment = TextAlignmentType.Center;
			headerStyle.Font.IsBold = true;

			// These are separate rows because the PDF isn't breaking lines
			ws.Cells.Merge(0, 0, 1, columnHeaders.Length);
			ws.Cells[0, 0].PutValue("Pennsylvania Cyber Charter School");
			ws.Cells[0, 0].SetStyle(headerStyle);

			ws.Cells.Merge(1, 0, 1, columnHeaders.Length);
			ws.Cells[1, 0].PutValue("Accounts Receivable Summary Report");
			ws.Cells[1, 0].SetStyle(headerStyle);

			ws.Cells.Merge(2, 0, 1, columnHeaders.Length);
			ws.Cells[2, 0].PutValue($"School Year {result.SchoolYear} as of {result.AsOf.ToString("MM/dd/yyyy")}");
			ws.Cells[2, 0].SetStyle(headerStyle);

			ws.Cells.SetRowHeightInch(3, 0.4);

			ws.Cells.StandardWidthInch = 1;
			ws.Cells.SetColumnWidthInch(0, 2);
			ws.Cells.SetColumnWidthInch(5, 0.75);

			var columnHeaderStyle = new CellsFactory().CreateStyle();
			columnHeaderStyle.Pattern = BackgroundType.Solid;
			columnHeaderStyle.ForegroundColor = Color.DarkGray;
			columnHeaderStyle.Font.IsBold = true;
			columnHeaderStyle.HorizontalAlignment = TextAlignmentType.Center;
			columnHeaderStyle.VerticalAlignment = TextAlignmentType.Center;
			columnHeaderStyle.IsTextWrapped = true;

			for (var i = 0; i < columnHeaders.Length; i++)
			{
				ws.Cells[3, i].PutValue(columnHeaders[i]);
				ws.Cells[3, i].SetStyle(columnHeaderStyle);
			}

			var numericSyle = new CellsFactory().CreateStyle();
			numericSyle.Custom = "#,##0.00_);(#,##0.00)";

			var paymentTypeStyle = new CellsFactory().CreateStyle();
			paymentTypeStyle.HorizontalAlignment = TextAlignmentType.Center;

			// TODO(Erik): this should really be done in the reporter
			var totalDue = 0m;
			var totalRefunded = 0m;
			var totalPaid = 0m;
			var totalNetDue = 0m;

			var r = 4;
			foreach (var district in result.SchoolDistricts)
			{
				var c = 0;
				ws.Cells[r, c++].PutValue(district.Name);
				ws.Cells[r, c++].PutValue(district.TotalDue);
				ws.Cells[r, c - 1].SetStyle(numericSyle);
				totalDue += district.TotalDue;
				ws.Cells[r, c++].PutValue(district.Refunded);
				ws.Cells[r, c - 1].SetStyle(numericSyle);
				totalRefunded += district.Refunded;
				ws.Cells[r, c++].PutValue(district.TotalPaid);
				ws.Cells[r, c - 1].SetStyle(numericSyle);
				totalPaid += district.TotalPaid;
				ws.Cells[r, c++].PutValue(district.NetDue);
				ws.Cells[r, c - 1].SetStyle(numericSyle);
				totalNetDue += district.NetDue;
				ws.Cells[r, c++].PutValue(district.PaymentType == SchoolDistrictPaymentType.Check.Value ? "SD" : "PDE");
				ws.Cells[r, c - 1].SetStyle(paymentTypeStyle);
				r++;
			}

			var totalsStyle = new CellsFactory().CreateStyle();
			totalsStyle.HorizontalAlignment = TextAlignmentType.Right;
			totalsStyle.Font.IsBold = true;
			ws.Cells[r, 0].PutValue("Totals:");
			ws.Cells[r, 0].SetStyle(totalsStyle);

			var totalsNumericStyle = new CellsFactory().CreateStyle();
			totalsNumericStyle.Copy(numericSyle);
			totalsNumericStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);

			ws.Cells[r, 1].PutValue(totalDue);
			ws.Cells[r, 1].SetStyle(totalsNumericStyle);
			ws.Cells[r, 2].PutValue(totalRefunded);
			ws.Cells[r, 2].SetStyle(totalsNumericStyle);
			ws.Cells[r, 3].PutValue(totalPaid);
			ws.Cells[r, 3].SetStyle(totalsNumericStyle);
			ws.Cells[r, 4].PutValue(totalNetDue);
			ws.Cells[r, 4].SetStyle(totalsNumericStyle);

			Report report;
			using (var xlsxStream = new MemoryStream())
			using (var pdfStream = new MemoryStream())
			{
				wb.CalculateFormula();
				wb.Save(xlsxStream, SaveFormat.Xlsx);
				wb.Save(pdfStream, SaveFormat.Pdf);

				report = new Report
				{
					Type = ReportType.AccountsReceivableAsOf,
					SchoolYear = create.SchoolYear,
					Name = create.Name,
					Approved = true,
					Created = DateTime.Now,
					Data = JsonConvert.SerializeObject(result),
					Xlsx = xlsxStream.ToArray(),
					Pdf = pdfStream.ToArray(),
				};
			}

			return report;
		}

		private Report CreateCsiu(CreateReport create)
		{
			var reporter = _reporters.CreateCsiuReporter(_context);
			var result = reporter.GenerateReport(new CsiuReporter.Config
			{
				AsOf = create.Csiu.AsOf,
				Auns = create.Csiu.Auns,
			});

			var wb = new Workbook();
			var ws = wb.Worksheets[0];

			var columnHeaders = new[] { "G/L Account #", "Amount", "Description" };
			var headerStyle = new CellsFactory().CreateStyle();
			headerStyle.HorizontalAlignment = TextAlignmentType.Center;
			headerStyle.Font.IsBold = true;
			headerStyle.Font.Underline = FontUnderlineType.Accounting;

			for (var i = 0; i < columnHeaders.Length; i++)
			{
				ws.Cells[0, i].PutValue(columnHeaders[i]);
				ws.Cells[0, i].SetStyle(headerStyle);
			}

			ws.Cells.SetColumnWidthInch(0, 2.5);
			ws.Cells.SetColumnWidthInch(1, 1);
			ws.Cells.SetColumnWidthInch(2, 2.5);

			var numericSyle = new CellsFactory().CreateStyle();
			numericSyle.Custom = "#,##0.00_);(#,##0.00)";

			var r = 1;
			foreach (var account in result.Accounts)
			{
				var c = 0;
				ws.Cells[r, c++].PutValue(account.Number);
				ws.Cells[r, c++].PutValue(account.Amount);
				ws.Cells[r, c - 1].SetStyle(numericSyle);
				ws.Cells[r, c++].PutValue(account.Description);
				r++;
			}

			Report report;
			using (var xlsxStream = new MemoryStream())
			using (var pdfStream = new MemoryStream())
			{
				wb.CalculateFormula();
				wb.Save(xlsxStream, SaveFormat.Xlsx);
				wb.Save(pdfStream, SaveFormat.Pdf);

				report = new Report
				{
					Type = ReportType.Csiu,
					SchoolYear = create.SchoolYear,
					Name = create.Name,
					Approved = true,
					Created = DateTime.Now,
					Data = JsonConvert.SerializeObject(result),
					Xlsx = xlsxStream.ToArray(),
					Pdf = pdfStream.ToArray(),
				};
			}

			return report;
		}

		private Report CreateUniPayInvoiceSummary(CreateReport create)
		{
			var reporter = _reporters.CreateUniPayInvoiceSummaryReporter(_context);
			var result = reporter.GenerateReport(new UniPayInvoiceSummaryReporter.Config
			{
				SchoolYear = create.SchoolYear,
				AsOf = create.UniPayInvoiceSummary.AsOf
			});
			var startYear = create.SchoolYear.Substring(0, 4);
			var endYear = create.SchoolYear.Substring(create.SchoolYear.IndexOf("-"));
			var wb = new Workbook();
			var ws = wb.Worksheets[0];

			ws.PageSetup.SetFooter(1, "Page &P of &N");
			ws.PageSetup.SetFooter(2, result.AsOf.ToString("M/dd/yyyy"));
			ws.PageSetup.HeaderMargin = 0.0;
			ws.PageSetup.FooterMargin = 0.0;
			ws.PageSetup.TopMargin = 0.0;
			ws.PageSetup.LeftMargin = 0.0;
			ws.PageSetup.RightMargin = 0.0;

			var columnHeaders = new[] { "SD AUN", "School District Name", "Total PDE Subsidy Deductions to Date", "Net Due to Charter School" };

			var headerStyle = new CellsFactory().CreateStyle();
			headerStyle.HorizontalAlignment = TextAlignmentType.Center;
			headerStyle.Font.IsBold = true;

			// These are separate rows because the PDF isn't breaking lines
			ws.Cells.Merge(0, 1, 1, columnHeaders.Length);
			System.Drawing.Color headerColor = System.Drawing.Color.FromArgb(128, 0, 0);
			headerStyle.Font.Color = headerColor; 
			ws.Cells[0, 1].SetStyle(headerStyle);
			ws.Cells[0, 1].PutValue("The Pennsylvania Cyber Charter School");
			ws.Cells[5, 4].SetStyle(headerStyle);
			ws.Cells[5, 4].PutValue($"{result.AsOf.ToString("M/dd/yyyy")}");

			ws.Cells.Merge(1, 1, 1, columnHeaders.Length);
			headerColor = System.Drawing.Color.FromArgb(0, 0, 0);
			headerStyle.Font.Color = headerColor;
			ws.Cells[1, 1].SetStyle(headerStyle);
			ws.Cells[1, 1].PutValue($"Summary of UniPay Request for the {result.SchoolYear} School Year");

			ws.Cells.Merge(2, 1, 1, columnHeaders.Length);
			ws.Cells[2, 1].PutValue($"For the Months of July {startYear} to {result.AsOf.ToString("MMMM yyyy")}");
			ws.Cells[2, 1].SetStyle(headerStyle);

			ws.Cells.Merge(3, 1, 1, columnHeaders.Length);
			ws.Cells[3, 1].PutValue($"Submission for {result.AsOf.ToString("MMMM")} {result.AsOf.Year} UniPay");
			ws.Cells[3, 1].SetStyle(headerStyle);

			ws.Cells.StandardWidthInch = 1;
			ws.Cells.SetColumnWidthInch(0, 1);
			ws.Cells.SetColumnWidthInch(1, 1);
			ws.Cells.SetColumnWidthInch(2, 2);

			var columnHeaderStyle = new CellsFactory().CreateStyle();
			columnHeaderStyle.Pattern = BackgroundType.Solid;
			columnHeaderStyle.ForegroundColor = Color.DarkGray;
			columnHeaderStyle.Font.IsBold = true;
			columnHeaderStyle.HorizontalAlignment = TextAlignmentType.Center;
			columnHeaderStyle.VerticalAlignment = TextAlignmentType.Center;
			columnHeaderStyle.IsTextWrapped = true;

			ws.Cells.SetRowHeightInch(6, 1);

			for (var i = 0; i < columnHeaders.Length; i++)
			{
				ws.Cells[6, i + 1].PutValue(columnHeaders[i]);
				ws.Cells[6, i + 1].SetStyle(columnHeaderStyle);
			}

			var numericStyle = new CellsFactory().CreateStyle();
			System.Drawing.Color numericForegroundColor = System.Drawing.Color.FromArgb(255, 255, 255, 153);
			numericStyle.Custom = "#,##0.00_);(#,##0.00)";
			numericStyle.ForegroundColor = numericForegroundColor;
			numericStyle.BackgroundColor = numericForegroundColor;
			numericStyle.Pattern = BackgroundType.Solid;

			var paymentTypeStyle = new CellsFactory().CreateStyle();
			paymentTypeStyle.HorizontalAlignment = TextAlignmentType.Center;

			// TODO(Erik): this should really be done in the reporter
			var totalPaid = 0m;
			var totalNetDue = 0m;

			var r = 7;
			foreach (var district in result.SchoolDistricts)
			{
				var c = 1;
				ws.Cells[r, c++].PutValue(district.Aun);
				ws.Cells[r, c++].PutValue(district.Name);
				ws.Cells[r, c++].PutValue(district.TotalPaid);
				totalPaid += district.TotalPaid;
				ws.Cells[r, c - 1].SetStyle(numericStyle);
				totalNetDue += district.NetDue;
				ws.Cells[r, c++].PutValue(district.NetDue);
				ws.Cells[r, c - 1].SetStyle(numericStyle);

				r++;
			}

			var totalsStyle = new CellsFactory().CreateStyle();
			totalsStyle.HorizontalAlignment = TextAlignmentType.Right;
			totalsStyle.Font.IsBold = true;
			ws.Cells[r, 0].PutValue("Totals:");
			ws.Cells[r, 0].SetStyle(totalsStyle);

			var totalsNumericStyle = new CellsFactory().CreateStyle();
			totalsNumericStyle.Copy(numericStyle);
			totalsNumericStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);

			ws.Cells[r, 3].PutValue(totalPaid);
			ws.Cells[r, 3].SetStyle(totalsNumericStyle);
			ws.Cells[r, 4].PutValue(totalNetDue);
			ws.Cells[r, 4].SetStyle(totalsNumericStyle);

			Report report;
			using (var xlsxStream = new MemoryStream())
			using (var pdfStream = new MemoryStream())
			{
				wb.CalculateFormula();
				wb.Save(xlsxStream, SaveFormat.Xlsx);
				wb.Save(pdfStream, SaveFormat.Pdf);

				report = new Report
				{
					Type = ReportType.UniPayInvoiceSummary,
					SchoolYear = create.SchoolYear,
					Name = create.Name,
					Approved = true,
					Created = DateTime.Now,
					Data = JsonConvert.SerializeObject(result),
					Xlsx = xlsxStream.ToArray(),
					Pdf = pdfStream.ToArray(),
				};
			}

			return report;
		}

		[HttpPost]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(ReportResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(404)]
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
				else if (create.ReportType == ReportType.BulkInvoice.Value)
				{
					if (create.BulkInvoice == null)
						return new BadRequestObjectResult(new ErrorsResponse("Cannot create bulk invoice without 'bulk invoice' config."));

					report = CreateBulkInvoice(create);
				}
				else if (create.ReportType == ReportType.StudentInformation.Value)
				{
					if (create.StudentInformation == null)
						return new BadRequestObjectResult(new ErrorsResponse("Cannot create student information without 'student information' config."));

					report = CreateStudentInformation(create);
				}
				else if (create.ReportType == ReportType.BulkStudentInformation.Value)
				{
					if (create.BulkStudentInformation == null)
						return new BadRequestObjectResult(new ErrorsResponse("Cannot create bulk student information without 'bulk student information' config."));

					report = CreateBulkStudentInformation(create);
				}
				else if (create.ReportType == ReportType.AccountsReceivableAsOf.Value)
				{
					if (create.AccountsReceivableAsOf == null)
						return new BadRequestObjectResult(new ErrorsResponse("Cannot create accounts receivable as of without 'accounts receivable as of' config."));

					report = CreateAccountsReceivableAsOf(create);
				}
				else if (create.ReportType == ReportType.Csiu.Value)
				{
					if (create.Csiu == null)
						return new BadRequestObjectResult(new ErrorsResponse("Cannot create CSIU without 'csiu' config."));

					report = CreateCsiu(create);
				}
				else if (create.ReportType == ReportType.UniPayInvoiceSummary.Value)
				{
					if (create.UniPayInvoiceSummary == null)
						return new BadRequestObjectResult(new ErrorResponse("Cannot create UniPay Invoice summary without 'unipay invoice summary' config."));

					report = CreateUniPayInvoiceSummary(create);
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
			catch (MissingCalendarException e)
			{
				return new BadRequestObjectResult(new ErrorsResponse(e));
			}
			catch (NotFoundException e)
			{
				Console.WriteLine(e.Message);
				return NotFound();
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

			public CreateManyInvoiceReports Invoice { get; set; }
		}

		private IList<Report> CreateManyInvoices(CreateManyReports create)
		{
			var invoiceTemplate = _templates.Get(ReportType.BulkInvoice, create.SchoolYear);
			if (invoiceTemplate == null)
				throw new MissingTemplateException(ReportType.BulkInvoice, create.SchoolYear);

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
						Invoice = new CreateInvoiceReport
						{
							Scope = create.Invoice.Scope,
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

		[HttpGet("{name}")]
		[Authorize(Policy = "PAY+")]
		[Produces(ContentTypes.XLSX, ContentTypes.PDF, ContentTypes.JSON)]
		[ProducesResponseType(404)]
		[ProducesResponseType(406)]
		public async Task<IActionResult> Get(string name)
		{
			var report = await Task.Run(() => _reports.Get(name));
			if (report == null)
				return NotFound();

			var accept = Request.Headers["Accept"];
			MemoryStream stream = null;
			foreach (var v in accept)
			{
				if (v.Contains(ContentTypes.XLSX))
				{
					stream = new MemoryStream(report.Xlsx);
					break;
				}
				else if (v.Contains(ContentTypes.PDF))
				{
					stream = new MemoryStream(report.Pdf);
					break;
				}
				else if (v.Contains(ContentTypes.JSON))
				{
					return new JsonResult(JsonConvert.DeserializeObject(report.Data));
				}
			}
			if (stream == null)
				return StatusCode(406);

			return new FileStreamResult(stream, accept) { FileDownloadName = report.Name };
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
