using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class ReportRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<ReportRepository> _logger;

		private ReportRepository _uut;

		private static SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");
		private static PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseLazyLoadingProxies().
				UseSqlite(_conn).Options);
			ctx.Database.Migrate();
			return ctx;
		}

		[SetUp]
		public void SetUp()
		{
			_conn.Open();
			_context = NewContext();
			_logger = new TestLogger<ReportRepository>();

			_uut = new ReportRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_conn.Close();
		}

		[Test]
		public void ApproveSetsApprovedTrue()
		{
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
				Approved = false,
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(report));

			_context.SaveChanges(() => _uut.Approve(report.Name));

			var actual = NewContext().Reports.Single();
			Assert.That(actual.Approved, Is.True);
		}

		[Test]
		public void ApproveThrowsNotFound()
		{
			Assert.That(() => _uut.Approve("bob"), Throws.TypeOf<NotFoundException>());
		}

		private static void AssertTemplate(Template actual, Template template)
		{
			Assert.That(actual.Id, Is.EqualTo(template.Id));
			Assert.That(actual.ReportType, Is.EqualTo(template.ReportType));
			Assert.That(actual.SchoolYear, Is.EqualTo(template.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(template.Name));
			Assert.That(actual.Created, Is.EqualTo(template.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(template.LastUpdated));
		}

		private static void AssertReport(Report actual, Report report)
		{
			Assert.That(actual.Id, Is.EqualTo(report.Id));
			Assert.That(actual.Type, Is.EqualTo(report.Type));
			Assert.That(actual.SchoolYear, Is.EqualTo(report.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(report.Name));
			Assert.That(actual.Approved, Is.EqualTo(report.Approved));
			Assert.That(actual.Created, Is.EqualTo(report.Created));
			Assert.That(actual.Data, Is.EqualTo(report.Data));
			AssertTemplate(actual.Template, report.Template);
		}

		private static void AssertReport(ReportMetadata actual, Report report)
		{
			Assert.That(actual.Id, Is.EqualTo(report.Id));
			Assert.That(actual.Type, Is.EqualTo(report.Type));
			Assert.That(actual.SchoolYear, Is.EqualTo(report.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(report.Name));
			Assert.That(actual.Approved, Is.EqualTo(report.Approved));
			Assert.That(actual.Created, Is.EqualTo(report.Created));
		}

		[Test]
		public void CreateCreates()
		{
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
				Data = "hello",
				Template = new Template
				{
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice-template",
				},
			};
			_context.SaveChanges(() => _context.Add(report.Template));

			var now = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.Create(now, report));
			Assert.That(result, Is.EqualTo(report));
			Assert.That(result.Created, Is.EqualTo(now));

			var actual = NewContext().Reports.Single();
			AssertReport(actual, report);
			Assert.That(actual.Created, Is.EqualTo(now));
		}

		[Test]
		public void CreateThrowsOnConflict()
		{
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
				Data = "hello",
				Template = new Template
				{
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice-template",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(report));

			Assert.That(() => _context.SaveChanges(() => _uut.Create(report)), Throws.TypeOf<DbUpdateException>());
		}

		[Test]
		public void GetGets()
		{
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
				Data = "hello",
				Template = new Template
				{
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice-template",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(report));

			var actual = _uut.Get(report.Name);
			AssertReport(actual, report);
		}

		[Test]
		public void GetReturnsNull()
		{
			var actual = _uut.Get("bob");
			Assert.That(actual, Is.Null);
		}

		[Test]
		public void GetManyMetadataReturnsAll()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
			};
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Template = template,
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Template = template,
				},
			};
			using (var ctx = NewContext())
			{
				ctx.Add(template);
				ctx.AddRange(reports);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetManyMetadata();
			Assert.That(actuals, Has.Count.EqualTo(reports.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertReport(actuals[i], reports[i]);
		}

		[Test]
		public void GetManyMetadataReturnsEmptyList()
		{
			var actuals = _uut.GetManyMetadata();
			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public void GetManyMetadataFiltersByName()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
			};
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Template = template,
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Template = template,
				},
			};
			using (var ctx = NewContext())
			{
				ctx.Add(template);
				ctx.AddRange(reports);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetManyMetadata(name: "invoice2");
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyMetadataFiltersByReportType()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
			};
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "hello",
					Template = template,
				},
				new Report {
					Type = ReportType.StudentInformation,
					SchoolYear = "2017-2018",
					Name = "student-info",
					Data = "hello",
					Template = template,
				},
			};
			using (var ctx = NewContext())
			{
				ctx.Add(template);
				ctx.AddRange(reports);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetManyMetadata(type: ReportType.StudentInformation);
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);

		}

		[Test]
		public void GetManyMetadataFiltersBySchoolYear()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
			};
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Template = template,
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2018-2019",
					Name = "invoice2",
					Data = "hello",
					Template = template,
				},
			};
			using (var ctx = NewContext())
			{
				ctx.Add(template);
				ctx.AddRange(reports);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetManyMetadata(year: "2018-2019");
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyMetadataFiltersByApproved()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
			};
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Template = template,
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Template = template,
					Approved = true,
				},
			};
			using (var ctx = NewContext())
			{
				ctx.Add(template);
				ctx.AddRange(reports);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetManyMetadata(approved: true);
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void RejectDeletes()
		{
			var report = new Report
			{
				Type = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "invoice",
				Data = "hello",
				Template = new Template
				{
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice-template",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(report));

			_context.SaveChanges(() => _uut.Reject(report.Name));

			var actual = NewContext().Reports.SingleOrDefault();
			Assert.That(actual, Is.Null);
		}

		[Test]
		public void RejectDoesNotThrowNotFound()
		{
			Assert.That(() => _uut.Reject("bob"), Throws.Nothing);
		}
	}
}
