using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NUnit.Framework;

using api.Common;
using api.Controllers;
using api.Dtos;
using api.Tests.Util;
using models;
using models.Reporters;
using models.Reporters.Exporters;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class ReportsControllerTests
	{
		private PacBillContext _context;
		private Mock<IReportRepository> _reports;
		private Mock<IReporterFactory> _reporters;
		private Mock<ITemplateRepository> _templates;
		private Mock<IXlsxExporter> _exporter;
		private Mock<ISchoolDistrictRepository> _districts;
		private ILogger<ReportsController> _logger;

		private ReportsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("reports-controller").Options);
			_reports = new Mock<IReportRepository>();
			_reporters = new Mock<IReporterFactory>();
			_templates = new Mock<ITemplateRepository>();
			_exporter = new Mock<IXlsxExporter>();
			_districts = new Mock<ISchoolDistrictRepository>();
			_logger = new TestLogger<ReportsController>();

			_uut = new ReportsController(
				_context,
				_reports.Object,
				_reporters.Object,
				_templates.Object,
				_exporter.Object,
				_districts.Object,
				_logger);
		}

		[TearDown]
		public void TearDown() => _context.Database.EnsureDeleted();

		private static bool VerifyReport(
			Report r,
			ReportsController.CreateReport create,
			XSSFWorkbook xlsx,
			Invoice invoice)
		{
			using (var ms = new MemoryStream())
			{
				xlsx.Write(ms);

				Console.WriteLine($"name: {r.Name} == {create.Name}");
				Console.WriteLine($"type: {r.Type} == {create.ReportType}");
				Console.WriteLine($"year: {r.SchoolYear} == {create.SchoolYear}");
				Console.WriteLine($"approved: {r.Approved} == false");
				Console.WriteLine($"data: {r.Data} == {JsonConvert.SerializeObject(invoice)}");
				Console.WriteLine($"xlsx: {r.Xlsx} == {ms.ToArray()}");
				return r.Name == create.Name &&
					r.Type == ReportType.FromString(create.ReportType) &&
					r.SchoolYear == create.SchoolYear &&
					r.Approved == false &&
					r.Data == JsonConvert.SerializeObject(invoice) &&
					r.Xlsx.SequenceEqual(ms.ToArray());
			}
		}

		private static bool VerifyReport(
			Report r,
			ReportsController.CreateManyReports create,
			XSSFWorkbook xlsx,
			Invoice invoice)
		{
			using (var ms = new MemoryStream())
			{
				xlsx.Write(ms);

				Console.WriteLine($"name: {r.Name} == {invoice.Number}");
				Console.WriteLine($"type: {r.Type} == {create.ReportType}");
				Console.WriteLine($"year: {r.SchoolYear} == {create.SchoolYear}");
				Console.WriteLine($"approved: {r.Approved} == false");
				Console.WriteLine($"data: {r.Data} == {JsonConvert.SerializeObject(invoice)}");
				Console.WriteLine($"xlsx: {r.Xlsx} == {ms.ToArray()}");
				return r.Name == invoice.Number &&
					r.Type == ReportType.FromString(create.ReportType) &&
					r.SchoolYear == create.SchoolYear &&
					r.Approved == false &&
					r.Data == JsonConvert.SerializeObject(invoice) &&
					r.Xlsx.SequenceEqual(ms.ToArray());
			}
		}

		private static bool MatchWorkbook(XSSFWorkbook wb, int expected)
		{
			Console.WriteLine($"num sheets: {wb.NumberOfSheets}");
			Console.WriteLine($"wb.Count: {wb.Count}");
			return wb.NumberOfSheets == expected;
		}

		[Test]
		public async Task CreateCreatesInvoice()
		{
			// build config
			var time = new DateTime(2018, 2, 1, 12, 30, 56);
			var create = new ReportsController.CreateReport
			{
				ReportType = ReportType.Invoice.Value,
				Name = "invoice",
				SchoolYear = "2017-2018",
				TemplateId = 3,
				Invoice = new ReportsController.CreateInvoiceReport
				{
					AsOf = time.Date,
					ToSchoolDistrict = time.Date.AddDays(9),
					ToPDE = time.Date.AddDays(20),
					SchoolDistrictAun = 123456789,
				},
			};

			// get reporter
			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).Returns(reporter.Object);

			// generate data
			var invoice = new Invoice
			{
				Number = create.Name,
				SchoolYear = create.SchoolYear,
				AsOf = create.Invoice.AsOf,
				Prepared = time,
				ToSchoolDistrict = create.Invoice.ToSchoolDistrict,
				ToPDE = create.Invoice.ToPDE,
				SchoolDistrict = new InvoiceSchoolDistrict
				{
					Aun = create.Invoice.SchoolDistrictAun,
					Name = "Some SD",
					RegularRate = 1000m,
					SpecialRate = 3000m,
				},
				RegularEnrollments = new InvoiceEnrollments
				{
					July = 3,
				},
				SpecialEnrollments = new InvoiceEnrollments
				{
					July = 3,
				},
				Transactions = new InvoiceTransactions
				{
					July = new InvoiceTransaction
					{
						Payment = new InvoicePayment
						{
							Type = PaymentType.Check.Value,
							CheckAmount = 10m,
							Date = time,
							CheckNumber = "1234",
						},
						Refund = 50m,
					}
				},
				Students = new List<InvoiceStudent> // trigger sheet cloning
				{
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
					new InvoiceStudent(),
				},
			};
			reporter.Setup(r => r.GenerateReport(It.Is<InvoiceReporter.Config>(c =>
					c.InvoiceNumber == create.Name &&
					c.SchoolYear == create.SchoolYear &&
					c.AsOf == create.Invoice.AsOf &&
					// c.Prepared == time && // TODO(Erik): DateTime.Now?
					c.ToSchoolDistrict == create.Invoice.ToSchoolDistrict &&
					c.ToPDE == create.Invoice.ToPDE &&
					c.SchoolDistrictAun == create.Invoice.SchoolDistrictAun
				))).Returns(invoice);

			// get templates
			using (var fs01 = File.OpenRead("../../../TestData/invoice-template.xlsx"))
			using (var ms01 = new MemoryStream())
			{
				fs01.CopyTo(ms01);
				var content01 = ms01.ToArray();

				var template = new Template
				{
					Id = create.TemplateId,
					Content = content01,
				};
				_templates.Setup(ts => ts.Get(template.Id)).Returns(template);
			}

			// generate xlsx
			XSSFWorkbook xlsx = null;
			_exporter.Setup(ex => ex.Export(
				It.Is<XSSFWorkbook>(wb => MatchWorkbook(wb, 3)),
				It.IsAny<JObject>()
			)).Returns<XSSFWorkbook, dynamic>((wb, _) =>
			{
				xlsx = wb;
				return wb;
			});

			// TODO(Erik): Something in VerifyReport occassionally fails.
			// save report
			Report report = null;
			_reports.Setup(rs => rs.Create(It.Is<Report>(r =>
				VerifyReport(r, create, xlsx, invoice)
			))).Returns<Report>(r =>
			{
				report = r;
				return r;
			});

			// return reportmetadata
			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			Assert.That(((CreatedResult)result).Location, Is.EqualTo($"/api/reports/{report.Name}"));
			var value = ((CreatedResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportResponse>());
			var actual = ((ReportsController.ReportResponse)value).Report;
			AssertReport(actual, report);
		}

		[Test]
		public async Task CreateInvoiceReturnsBadRequestIfMissingInvoice()
		{
			var result = await _uut.Create(new ReportsController.CreateReport
			{
				ReportType = ReportType.Invoice.Value,
			});
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals[0], Is.EqualTo("Cannot create invoice without 'invoice' config."));
		}

		[Test]
		public async Task CreateReturnsNotImplementedForUnimplementedReportType()
		{
			// TODO(Erik): implement all ReportTypes; delete this
			var result = await _uut.Create(new ReportsController.CreateReport
			{
				ReportType = ReportType.StudentInformation.Value,
			});
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(501));
		}

		[Test]
		public async Task CreateReturnsFailedDependencyIfTemplateNotFound()
		{
			var templateId = 1;
			_templates.Setup(ts => ts.Get(templateId)).Returns<Template>(null);

			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).Returns(reporter.Object);

			var result = await _uut.Create(new ReportsController.CreateReport
			{
				ReportType = ReportType.Invoice.Value,
				TemplateId = templateId,
				Invoice = new ReportsController.CreateInvoiceReport(),
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(424));
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var actual = ((ErrorResponse)value).Error;

			Assert.That(actual, Is.EqualTo($"Could not find template with Id '{templateId}'."));
		}

		[Test]
		public async Task CreateReturnsBadRequestWhenModelStateInvalid()
		{
			var key = "error";
			var msg = "borked";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.Create(new ReportsController.CreateReport());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task CreateReturnsConflict()
		{
			_reports.Setup(rs => rs.Create(It.IsAny<Report>())).Throws(new DbUpdateException("", new Exception()));

			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).Returns(reporter.Object);

			var invoice = new Invoice
			{
				SchoolYear = "2017-2018",
				Students = new[] {
					new InvoiceStudent(),
				},
			};
			reporter.Setup(r => r.GenerateReport(It.IsAny<InvoiceReporter.Config>())).Returns(invoice);

			using (var fs = File.OpenRead("../../../TestData/invoice-template.xlsx"))
			using (var ms = new MemoryStream())
			{
				fs.CopyTo(ms);
				_templates.Setup(ts => ts.Get(0)).Returns(new Template
				{
					Content = ms.ToArray(),
				});
			}

			_exporter.Setup(ex => ex.Export(
				It.IsAny<XSSFWorkbook>(),
				It.IsAny<JObject>()
			)).Returns<XSSFWorkbook, dynamic>((wb, _) => wb);

			var result = await _uut.Create(new ReportsController.CreateReport
			{
				ReportType = ReportType.Invoice.Value,
				Invoice = new ReportsController.CreateInvoiceReport(),
			});
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			var code = ((StatusCodeResult)result).StatusCode;

			Assert.That(code, Is.EqualTo(409));
		}

		private static bool MatchConfig(
			InvoiceReporter.Config config,
			ReportsController.CreateManyReports create,
			int aun,
			Invoice invoice)
		{
			Console.WriteLine($"number: {config.InvoiceNumber} == {create.SchoolYear}_{aun}_???");
			Console.WriteLine($"schoolYear: {config.SchoolYear} == {create.SchoolYear}");
			Console.WriteLine($"asOf: {config.AsOf} == {create.Invoice.AsOf}");
			Console.WriteLine($"toSchoolDistrict: {config.ToSchoolDistrict} == {create.Invoice.ToSchoolDistrict}");
			Console.WriteLine($"toPDE: {config.ToPDE} == {create.Invoice.ToPDE}");
			Console.WriteLine($"aun: {config.SchoolDistrictAun} == {aun}");
			return config.InvoiceNumber.StartsWith($"{create.SchoolYear}_{aun}_") && // TODO(erik): inject time?
				config.SchoolYear == create.SchoolYear &&
				config.AsOf == create.Invoice.AsOf &&
				config.ToSchoolDistrict == create.Invoice.ToSchoolDistrict &&
				config.ToPDE == create.Invoice.ToPDE &&
				config.SchoolDistrictAun == aun;
		}

		[Test]
		public void CreateManyCreatesInvoices()
		{
			// get all schooldistrict AUNs
			var auns = new[] {
				123456789,
				234567890,
				345678901
			};
//			_districts.Setup(ds => ds.GetManyAuns()).Returns(auns).Verifiable();

      var names = new[] {
        "Test School 1 SD",
        "Test School 2 SD",
        "Test School 3 SD"
      };
      _districts.Setup(ds => ds.GetManyNames()).Returns(names).Verifiable();

			// get reporter
			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).
				Returns(reporter.Object).Verifiable();

			// build config
			var time = new DateTime(2018, 02, 1);
			var create = new ReportsController.CreateManyReports
			{
				ReportType = ReportType.Invoice.Value,
				SchoolYear = "2017-2018",
				TemplateId = 1,
				Invoice = new ReportsController.CreateManyInvoiceReports
				{
					AsOf = time,
					ToSchoolDistrict = time.AddDays(9),
					ToPDE = time.AddDays(20),
				}
			};

			// generate data
			var invoices = new[] {
				new Invoice {
					SchoolYear = "2017-2018",
					SchoolDistrict = new InvoiceSchoolDistrict
					{
						Aun = auns[0],
            Name = names[0],
					},
					Students = new[] {
						new InvoiceStudent(),
					},
				},
				new Invoice {
					SchoolYear = "2017-2018",
					SchoolDistrict = new InvoiceSchoolDistrict
					{
						Aun = auns[1],
            Name = names[1],
					},
					Students = new[] {
						new InvoiceStudent(),
					},
				},
				new Invoice {
					SchoolYear = "2017-2018",
					SchoolDistrict = new InvoiceSchoolDistrict
					{
						Aun = auns[2],
            Name = names[2],
					},
					Students = new[] {
						new InvoiceStudent(),
					},
				},
			};
			// for (var i = 0; i < auns.Length; i++)
			// 	reporter.Setup(r => r.GenerateReport(It.Is<InvoiceReporter.Config>(c =>
			// 		MatchConfig(c, create, auns[i], invoices[i])
			// 	))).Returns(invoices[i]);

			reporter.Setup(r => r.GenerateReport(It.Is<InvoiceReporter.Config>(c =>
				MatchConfig(c, create, auns[0], invoices[0])
			))).Returns(invoices[0]).Verifiable();
			reporter.Setup(r => r.GenerateReport(It.Is<InvoiceReporter.Config>(c =>
				MatchConfig(c, create, auns[1], invoices[1])
			))).Returns(invoices[1]).Verifiable();
			reporter.Setup(r => r.GenerateReport(It.Is<InvoiceReporter.Config>(c =>
				MatchConfig(c, create, auns[2], invoices[2])
			))).Returns(invoices[2]).Verifiable();

			// get template
			using (var fs01 = File.OpenRead("../../../TestData/invoice-template.xlsx"))
			using (var ms01 = new MemoryStream())
			{
				fs01.CopyTo(ms01);
				var content01 = ms01.ToArray();

				var template = new Template
				{
					Id = create.TemplateId,
					Content = content01,
				};
				_templates.Setup(ts => ts.Get(template.Id)).Returns(template);
			}

			// generate xlsx
			var xlsxs = new List<XSSFWorkbook>();
			_exporter.Setup(ex => ex.Export(
				It.Is<XSSFWorkbook>(wb => MatchWorkbook(wb, 2)),
				It.IsAny<JObject>()
			)).Returns<XSSFWorkbook, dynamic>((wb, _) =>
			{
				xlsxs.Add(wb);
				return wb;
			}).Verifiable();

			// save report
			IList<Report> reports = null;
			_reports.Setup(rs => rs.CreateMany(It.IsAny<IList<Report>>())).Returns<IList<Report>>(rl =>
			{
				reports = rl.ToList();
				return rl;
			}).Verifiable();

			// return reportmetadata
			var result = _uut.CreateMany(create);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			Assert.That(((CreatedResult)result).Location, Is.EqualTo($"/api/reports?type={create.ReportType}&schoolYear={create.SchoolYear}&approved=false"));
			var value = ((CreatedResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportsResponse>());
			var actuals = ((ReportsController.ReportsResponse)value).Reports;
      // WDM 06-25-2018:  This needs fixed.
//			Assert.That(actuals, Has.Count.EqualTo(auns.Length));
//			for (var i = 0; i < auns.Length; i++)
//				AssertReport(actuals[i], reports[i]);

			_districts.Verify();
//			_reporters.Verify();
//			reporter.Verify();
//			_exporter.Verify();
//			_reporters.Verify();
		}

		[Test]
		public void CreateManyInvoicesReturnsReturnsBadRequestWhenMissingInvoice()
		{
			var create = new ReportsController.CreateManyReports
			{
				ReportType = ReportType.Invoice.Value,
			};

			var result = _uut.CreateMany(create);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals[0], Is.EqualTo("Cannot create invoice without 'invoice' config."));
		}

		[Test]
		public void CreateManyReturnsNotImplementedForUnimplementedReportType()
		{
			// TODO(Erik): implement all ReportTypes; delete this
			var result = _uut.CreateMany(new ReportsController.CreateManyReports
			{
				ReportType = ReportType.StudentInformation.Value,
			});
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(501));
		}

		[Test]
		public void CreateManyReturnsFailedDependencyIfTemplateNotFound()
		{
			var templateId = 1;
			_templates.Setup(ts => ts.Get(templateId)).Returns<Template>(null);

			_districts.Setup(ds => ds.GetManyAuns()).Returns(new[] { 123456789 });

			var invoice = new Invoice();
			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			reporter.Setup(r => r.GenerateReport(It.IsAny<InvoiceReporter.Config>())).Returns(invoice);
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).Returns(reporter.Object);

			var result = _uut.CreateMany(new ReportsController.CreateManyReports
			{
				ReportType = ReportType.Invoice.Value,
				TemplateId = templateId,
				Invoice = new ReportsController.CreateManyInvoiceReports(),
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(424));
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var actual = ((ErrorResponse)value).Error;

			Assert.That(actual, Is.EqualTo($"Could not find template with Id '{templateId}'."));
		}

		[Test]
		public void CreateManyReturnsBadRequestWhenModelStateInvalid()
		{
			var key = "error";
			var msg = "borked";
			_uut.ModelState.AddModelError(key, msg);

			var result = _uut.CreateMany(new ReportsController.CreateManyReports());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals[0], Is.EqualTo(msg));
		}

		[Test]
		public void CreateManyReturnsConflict()
		{
			_reports.Setup(rs => rs.CreateMany(It.IsAny<IList<Report>>())).Throws(new DbUpdateException("", new Exception()));

			/* _districts.Setup(ds => ds.GetManyAuns()).Returns(new[] { 123456789 }); */
      _districts.Setup(ds => ds.GetManyNames()).Returns(new[] { "Test School SD" });

			var reporter = new Mock<IReporter<Invoice, InvoiceReporter.Config>>();
			_reporters.Setup(rs => rs.CreateInvoiceReporter(_context)).Returns(reporter.Object);

			var invoice = new Invoice
			{
				SchoolYear = "2017-2018",
				Students = new[] {
					new InvoiceStudent(),
				},
			};
			reporter.Setup(r => r.GenerateReport(It.IsAny<InvoiceReporter.Config>())).Returns(invoice);

			using (var fs = File.OpenRead("../../../TestData/invoice-template.xlsx"))
			using (var ms = new MemoryStream())
			{
				fs.CopyTo(ms);
				_templates.Setup(ts => ts.Get(0)).Returns(new Template
				{
					Content = ms.ToArray(),
				});
			}

			_exporter.Setup(ex => ex.Export(
				It.IsAny<XSSFWorkbook>(),
				It.IsAny<JObject>()
			)).Returns<XSSFWorkbook, dynamic>((wb, _) => wb);


			var result = _uut.CreateMany(new ReportsController.CreateManyReports
			{
				ReportType = ReportType.Invoice.Value,
				Invoice = new ReportsController.CreateManyInvoiceReports(),
			});
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			var code = ((StatusCodeResult)result).StatusCode;

			Assert.That(code, Is.EqualTo(409));
		}

		[Test]
		public async Task GetGetsXlsx()
		{
			var templateFileStream = File.OpenRead("../../../TestData/invoice-template.xlsx");
			Report report;
			using (var templateMemoryStream = new MemoryStream())
			{
				templateFileStream.CopyTo(templateMemoryStream);

				report = new Report
				{
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "{'hello':'world'}",
					Xlsx = templateMemoryStream.ToArray(),
				};
			}
			_reports.Setup(rs => rs.Get(report.Name)).Returns(report);

			_uut.SetHeader("Accept", ContentTypes.XLSX);

			var result = await _uut.Get(report.Name);
			Assert.That(result, Is.TypeOf<FileStreamResult>());
			var file = (FileStreamResult)result;

			Assert.That(file.FileDownloadName, Is.EqualTo(report.Name));
			Assert.That(file.ContentType, Is.EqualTo(ContentTypes.XLSX));

			var resultFileStream = file.FileStream;
			using (var resultMemoryStream = new MemoryStream())
			{
				resultFileStream.CopyTo(resultMemoryStream);
				Assert.That(resultMemoryStream.ToArray(), Is.EqualTo(report.Xlsx));
			}
		}

		[Test]
		public async Task GetReturnsNotFound()
		{
			var name = "bob";
			_reports.Setup(rs => rs.Get(name)).Returns<Report>(null);

			var result = await _uut.Get(name);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task GetReturnsNotAcceptableWhenAcceptHeaderInvalid()
		{
			_uut.SetHeader("Accept", "bob");
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
			};
			_reports.Setup(rs => rs.Get(report.Name)).Returns(report);

			var result = await _uut.Get(report.Name);
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(406));
		}

		private static void AssertReport(ReportDto actual, Report report)
		{
			Assert.That(actual.Id, Is.EqualTo(report.Id));
			Assert.That(actual.Type, Is.EqualTo(report.Type));
			Assert.That(actual.SchoolYear, Is.EqualTo(report.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(report.Name));
			Assert.That(actual.Approved, Is.EqualTo(report.Approved));
			Assert.That(actual.Created, Is.EqualTo(report.Created));
		}

		[Test]
		public async Task GetManyMetadataNoArgsReturnsList()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
				},
			};
			_reports.Setup(rs => rs.GetManyMetadata(null, null, null, null)).Returns(reports);

			var result = await _uut.GetManyMetadata(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportsResponse>());
			var actuals = ((ReportsController.ReportsResponse)value).Reports;

			Assert.That(actuals, Has.Count.EqualTo(reports.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertReport(actuals[i], reports[i]);
		}

		[Test]
		public async Task GetManyMetadataAllArgsReturnsList()
		{
			var name = "invoice";
			var type = ReportType.Invoice;
			var year = "2017-2018";
			var approved = true;
			var reports = new[] {
				new Report {
					Type = type,
					SchoolYear = year,
					Name = name,
					Approved = approved,
				},
				new Report {
					Type = type,
					SchoolYear = year,
					Name = name,
					Approved = approved,
				},
			};
			_reports.Setup(rs => rs.GetManyMetadata(name, type, year, approved)).Returns(reports);

			var result = await _uut.GetManyMetadata(new ReportsController.GetManyArgs
			{
				Name = name,
				Type = type.Value,
				SchoolYear = year,
				Approved = approved,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportsResponse>());
			var actuals = ((ReportsController.ReportsResponse)value).Reports;

			Assert.That(actuals, Has.Count.EqualTo(reports.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertReport(actuals[i], reports[i]);
		}

		[Test]
		public async Task GetManyMetadataReturnsBadRequestWhenModelStateInvalid()
		{
			var key = "err";
			var msg = "borked";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetManyMetadata(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var errors = ((ErrorsResponse)value).Errors;

			Assert.That(errors[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task GetManyMetadataReturnsEmptyListWhenEmpty()
		{
			_reports.Setup(rs => rs.GetManyMetadata(null, null, null, null)).Returns(new List<ReportMetadata>());

			var result = await _uut.GetManyMetadata(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportsResponse>());
			var actuals = ((ReportsController.ReportsResponse)value).Reports;

			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public async Task GetManyMetadataReturnsEmptyListWhenNull()
		{
			_reports.Setup(rs => rs.GetManyMetadata(null, null, null, null)).Returns<List<ReportMetadata>>(null);

			var result = await _uut.GetManyMetadata(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ReportsController.ReportsResponse>());
			var actuals = ((ReportsController.ReportsResponse)value).Reports;

			Assert.That(actuals, Is.Empty);
		}

		private static void AssertContains(byte[] actual, byte[] expected)
		{
			var e = 0;
			for (var a = 0; a < actual.Length; a++)
			{
				if (actual[a] == expected[e])
				{
					if (e == expected.Length - 1)
					{
						Assert.Pass();
						return;
					}

					e++;
					continue;
				}

				e = 0;
			}

			Assert.Fail("Actual sequence did not contain expected sequence.");
		}

		[Test]
		public async Task GetZipNoArgsReturnsZip()
		{
			var reports = new[] {
				new Report {
					Name = "invoice",
					Xlsx = Encoding.UTF8.GetBytes("hello"),
				},
			};
			_reports.Setup(rs => rs.GetMany(null, null, null, null)).Returns(reports);

			var result = await _uut.GetZip(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<FileStreamResult>());
			var file = (FileStreamResult)result;

			Assert.That(file.ContentType, Is.EqualTo(ContentTypes.ZIP));
			Assert.That(file.FileDownloadName, Is.EqualTo("Reports"));

			using (var ms = new MemoryStream())
			{
				file.FileStream.CopyTo(ms);
				var archive = new ZipArchive(ms);
				Assert.That(archive.Entries, Has.Count.EqualTo(reports.Length));
				for (var i = 0; i < archive.Entries.Count; i++)
				{
					using (var entryStream = new MemoryStream())
					{
						archive.Entries[i].Open().CopyTo(ms);
						Console.WriteLine($"entry: {Encoding.UTF8.GetString(ms.ToArray())}");
						AssertContains(ms.ToArray(), reports[i].Xlsx);
					}
				}
			}
		}

		[Test]
		public async Task GetZipAllArgsReturnsZip()
		{
			var reports = new[] {
				new Report {
					Name = "invoice",
					Xlsx = Encoding.UTF8.GetBytes("hello"),
				},
			};

			var name = "bob";
			var type = ReportType.Invoice;
			var schoolYear = "2017-2018";
			var approved = true;
			_reports.Setup(rs => rs.GetMany(name, type, schoolYear, approved)).Returns(reports);

			var result = await _uut.GetZip(new ReportsController.GetManyArgs
			{
				Name = name,
				Type = type.Value,
				SchoolYear = schoolYear,
				Approved = approved,
			});
			Assert.That(result, Is.TypeOf<FileStreamResult>());
			var file = (FileStreamResult)result;

			Assert.That(file.ContentType, Is.EqualTo(ContentTypes.ZIP));
			Assert.That(file.FileDownloadName, Is.EqualTo($"Reports-{schoolYear}-{type}-{name}-Approved"));

			using (var ms = new MemoryStream())
			{
				file.FileStream.CopyTo(ms);
				var archive = new ZipArchive(ms);
				Assert.That(archive.Entries, Has.Count.EqualTo(reports.Length));
				for (var i = 0; i < archive.Entries.Count; i++)
				{
					using (var entryStream = new MemoryStream())
					{
						archive.Entries[i].Open().CopyTo(ms);
						AssertContains(ms.ToArray(), reports[i].Xlsx);
					}
				}
			}
		}

		[Test]
		public async Task GetZipReturnsNoContentWhenNull()
		{
			_reports.Setup(rs => rs.GetMany(null, null, null, null)).Returns<IEnumerable<Report>>(null);

			var result = await _uut.GetZip(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<NoContentResult>());
		}

		[Test]
		public async Task GetZipReturnsNoContentWhenEmpty()
		{
			_reports.Setup(rs => rs.GetMany(null, null, null, null)).Returns(new Report[] { });

			var result = await _uut.GetZip(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<NoContentResult>());
		}

		[Test]
		public async Task GetZipReturnBadRequestWhenModelStateInvalid()
		{
			var key = "err";
			var msg = "borked";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetZip(new ReportsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task ApproveApproves()
		{
			var name = "bob";
			_reports.Setup(rs => rs.Approve(name)).Verifiable();

			var result = await _uut.Approve(name);
			Assert.That(result, Is.TypeOf<OkResult>());

			_reports.Verify();
		}

		[Test]
		public async Task ApproveReturnsNotFound()
		{
			var name = "bob";
			_reports.Setup(rs => rs.Approve(name)).Throws(new NotFoundException());

			var result = await _uut.Approve(name);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task RejectRejects()
		{
			var name = "bob";
			_reports.Setup(rs => rs.Reject(name)).Verifiable();

			var result = await _uut.Reject(name);
			Assert.That(result, Is.TypeOf<OkResult>());

			_reports.Verify();
		}
	}
}
