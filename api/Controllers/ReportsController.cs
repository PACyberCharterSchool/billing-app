using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Swashbuckle.AspNetCore.SwaggerGen;

using api.Common;
using api.Dtos;
using models;
using models.Reporters;
using models.Reporters.Exporters;

namespace api.Controllers
{
	// TODO(Erik): auth!
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

						if (r == 12 && c == 1) // Number column
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

		// TODO(Erik): page # in footer
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
			var auns = _districts.GetManyAuns();
			foreach (var aun in auns)
				reports.Add(CreateInvoice(now, invoiceTemplate, new CreateReport
				{
					ReportType = create.ReportType,
					Name = $"{create.SchoolYear}_{aun}_{now.ToSecondsFromEpoch()}",
					SchoolYear = create.SchoolYear,
					TemplateId = create.TemplateId,

					Invoice = new CreateInvoiceReport
					{
						AsOf = create.Invoice.AsOf,
						ToSchoolDistrict = create.Invoice.ToSchoolDistrict,
						ToPDE = create.Invoice.ToPDE,
						SchoolDistrictAun = aun,
					},
				}));

			return reports;
		}

		private static readonly object _lock = new object();

		[HttpPost("many")]
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
		[Produces(ContentTypes.XLSX)]
		[ProducesResponseType(404)]
		[ProducesResponseType(406)]
		public async Task<IActionResult> Get(string name)
		{
			var report = await Task.Run(() => _reports.Get(name));
			if (report == null)
				return NotFound();

			var accept = Request.Headers["Accept"];
			if (accept != ContentTypes.XLSX)
				return StatusCode(406);

			var stream = new MemoryStream(report.Xlsx);
			return new FileStreamResult(stream, ContentTypes.XLSX)
			{
				FileDownloadName = report.Name,
			};
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

		[HttpGet]
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
		[ProducesResponseType(200)]
		public async Task<IActionResult> Reject(string name)
		{
			await Task.Run(() => _context.SaveChanges(() => _reports.Reject(name)));
			return Ok();
		}
	}
}
