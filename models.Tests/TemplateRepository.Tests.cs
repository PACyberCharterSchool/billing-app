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
		public void GetGets()
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
		public void GetReturnsNullIfNotFound()
		{
			var actual = _uut.Get(ReportType.Invoice, "2017-2018");
			Assert.That(actual, Is.Null);
		}
	}
}
