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
	public class TemplateRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<TemplateRepository> _logger;

		private TemplateRepository _uut;

		private readonly SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");
		private PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().UseSqlite(_conn).Options);
			ctx.Database.Migrate();
			return ctx;
		}

		[SetUp]
		public void SetUp()
		{
			_conn.Open();
			_context = NewContext();
			_logger = new TestLogger<TemplateRepository>();

			_uut = new TemplateRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_conn.Close();
		}

		public static void AssertTemplate(Template actual, Template template)
		{
			Assert.That(actual.Id, Is.EqualTo(template.Id));
			Assert.That(actual.ReportType, Is.EqualTo(template.ReportType));
			Assert.That(actual.SchoolYear, Is.EqualTo(template.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(template.Name));
			Assert.That(actual.Content, Is.EqualTo(template.Content));
		}

		public static void AssertTemplate(TemplateMetadata actual, Template template)
		{
			Assert.That(actual.Id, Is.EqualTo(template.Id));
			Assert.That(actual.ReportType, Is.EqualTo(template.ReportType));
			Assert.That(actual.SchoolYear, Is.EqualTo(template.SchoolYear));
			Assert.That(actual.Name, Is.EqualTo(template.Name));
		}

		[Test]
		public void CreateOrUpdateWithNewObjectCreates()
		{
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
			};

			var now = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(now, template));
			Assert.That(result, Is.EqualTo(template));

			var actual = NewContext().Templates.First(t => t.Id == template.Id);
			AssertTemplate(actual, template);
			Assert.That(actual.Created, Is.EqualTo(now));
			Assert.That(actual.LastUpdated, Is.EqualTo(now));
		}

		[Test]
		public void CreateOrUpdateWithSameObjectUpdates()
		{
			var then = DateTime.Now.AddHours(-1);
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
			};
			_context.SaveChanges(() => _uut.CreateOrUpdate(then, template));

			template.Content = Encoding.UTF8.GetBytes("goodbye");

			var now = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(now, template));
			Assert.That(result, Is.EqualTo(template));

			var actual = NewContext().Templates.First(t => t.Id == template.Id);
			AssertTemplate(actual, template);
			Assert.That(actual.Created, Is.EqualTo(then));
			Assert.That(actual.LastUpdated, Is.EqualTo(now));
		}

		[Test]
		public void CreateOrUpdateWithDifferentObjectUpdates()
		{
			var then = DateTime.Now.AddHours(-1);
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
				Created = then,
				LastUpdated = then,
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(template));

			var update = new Template
			{
				Id = template.Id,
				ReportType = template.ReportType,
				SchoolYear = template.SchoolYear,
				Name = template.Name,
				Content = Encoding.UTF8.GetBytes("goodbye"),
			};

			var now = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(now, update));
			AssertTemplate(result, update);

			var actual = NewContext().Templates.First(t => t.Id == template.Id);
			AssertTemplate(actual, update);
			Assert.That(actual.Created, Is.EqualTo(then));
			Assert.That(actual.LastUpdated, Is.EqualTo(now));
		}

		[Test]
		public void GetByIdGets()
		{
			var time = DateTime.Now;
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
				Created = time,
				LastUpdated = time,
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(template));

			var actual = _uut.Get(template.Id);
			AssertTemplate(actual, template);
		}

		[Test]
		public void GetByIdReturnsNullIfNotFound()
		{
			var actual = _uut.Get(1);
			Assert.That(actual, Is.Null);
		}

		[Test]
		public void GetByReportTypeAndSchoolYearGets()
		{
			var time = DateTime.Now;
			var template = new Template
			{
				ReportType = ReportType.Invoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
				Created = time,
				LastUpdated = time,
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.Add(template));

			var actual = _uut.Get(template.ReportType, template.SchoolYear);
			AssertTemplate(actual, template);
		}

		[Test]
		public void GetByReportTypeAndSchoolYearReturnsNullIfNotFound()
		{
			var actual = _uut.Get(ReportType.Invoice, "2017-2018");
			Assert.That(actual, Is.Null);
		}

		[Test]
		public void GetManyMetadataReturnsAll()
		{
			var templates = new[] {
				new Template {
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "template1",
				},
				new Template {
					ReportType = ReportType.Invoice,
					SchoolYear = "2018-2019",
					Name = "template2",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(templates));

			var actual = _uut.GetManyMetadata();
			Assert.That(actual, Has.Count.EqualTo(templates.Length));
			for (var i = 0; i < actual.Count; i++)
				AssertTemplate(actual[i], templates[i]);
		}

		[Test]
		public void GetManyMetadataReturnsEmptyListWhenEmpty()
		{
			var actual = _uut.GetManyMetadata();
			Assert.That(actual, Is.Empty);
		}

		[Test]
		public void GetManyMetadataFiltersByReportType()
		{
			var templates = new[] {
				new Template {
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "template",
				},
				new Template {
					ReportType = ReportType.StudentInformation,
					SchoolYear = "2017-2018",
					Name = "template",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(templates));

			var actual = _uut.GetManyMetadata(type: ReportType.Invoice);
			Assert.That(actual, Has.Count.EqualTo(1));
			AssertTemplate(actual[0], templates[0]);
		}

		[Test]
		public void GetManyMetadataFiltersBySchoolYear()
		{
			var templates = new[] {
				new Template {
					ReportType = ReportType.Invoice,
					SchoolYear = "2017-2018",
					Name = "template",
				},
				new Template {
					ReportType = ReportType.Invoice,
					SchoolYear = "2018-2019",
					Name = "template",
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(templates));

			var actual = _uut.GetManyMetadata(year: "2018-2019");
			Assert.That(actual, Has.Count.EqualTo(1));
			AssertTemplate(actual[0], templates[1]);
		}
	}
}
