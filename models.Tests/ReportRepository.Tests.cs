using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	[Ignore("Cause tests suck")]
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

		private static void AssertReport(ReportMetadata actual, Report report)
		{
			Assert.That(actual.Id, Is.EqualTo(report.Id));
			Assert.That(actual.Type, Is.EqualTo(report.Type));
			Assert.That(actual.SchoolYear, Is.EqualTo(report.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(report.Name));
			Assert.That(actual.Approved, Is.EqualTo(report.Approved));
			Assert.That(actual.Created, Is.EqualTo(report.Created));
		}

		private static void AssertReport(Report actual, Report report)
		{
			AssertReport(actual as ReportMetadata, report);
			Assert.That(actual.Data, Is.EqualTo(report.Data));
			Assert.That(actual.Xlsx, Is.EqualTo(report.Xlsx));
		}

		[Test]
		public void CreateManyCreates()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "hello",
					Xlsx = Encoding.UTF8.GetBytes("hello"),
				},
			};

			var now = DateTime.Now;
			var results = _context.SaveChanges(() => _uut.CreateMany(now, reports));
			Assert.That(results, Is.EqualTo(reports));

			var actuals = NewContext().Reports.ToList();
			Assert.That(actuals, Has.Count.EqualTo(reports.Length));
			for (var i = 0; i < actuals.Count; i++)
			{
				AssertReport(actuals[i], reports[i]);
				Assert.That(actuals[0].Created, Is.EqualTo(now));
			}
		}

		[Test]
		public void CreateManyThrowsOnConflict()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "hello",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(reports)), Throws.TypeOf<DbUpdateException>());
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
				Xlsx = Encoding.UTF8.GetBytes("hello"),
			};

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
		public void GetManyReturnsAll()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetMany().ToList();
			Assert.That(actuals, Has.Count.EqualTo(reports.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertReport(actuals[i], reports[i]);
		}

		[Test]
		public void GetManyReturnsEmptyList()
		{
			var actuals = _uut.GetMany().ToList();
			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public void GetManyFiltersByName()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetMany(name: "invoice2").ToList();
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyFiltersByReportType()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
				new Report {
					Type = ReportType.StudentInformation,
					SchoolYear = "2017-2018",
					Name = "student-info",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetMany(type: ReportType.StudentInformation).ToList();
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);

		}

		[Test]
		public void GetManyFiltersBySchoolYear()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2018-2019",
					Name = "invoice2",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetMany(year: "2018-2019").ToList();
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyFiltersByApproved()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
					Xlsx = new byte[] {1, 2, 3},
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Approved = true,
					Xlsx = new byte[] {1, 2, 3},
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetMany(approved: true).ToList();
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyMetadataReturnsAll()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

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
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetManyMetadata(name: "invoice2");
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyMetadataFiltersByReportType()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice",
					Data = "hello",
				},
				new Report {
					Type = ReportType.StudentInformation,
					SchoolYear = "2017-2018",
					Name = "student-info",
					Data = "hello",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetManyMetadata(type: ReportType.StudentInformation);
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);

		}

		[Test]
		public void GetManyMetadataFiltersBySchoolYear()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2018-2019",
					Name = "invoice2",
					Data = "hello",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

			var actuals = _uut.GetManyMetadata(year: "2018-2019");
			Assert.That(actuals, Has.Count.EqualTo(1));
			AssertReport(actuals[0], reports[1]);
		}

		[Test]
		public void GetManyMetadataFiltersByApproved()
		{
			var reports = new[] {
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice1",
					Data = "hello",
				},
				new Report {
					Type = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "invoice2",
					Data = "hello",
					Approved = true,
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(reports));

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
