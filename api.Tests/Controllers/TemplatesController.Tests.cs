using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Tests.Util;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class TemplatesControllerTests
	{
		private PacBillContext _context;
		private Mock<ITemplateRepository> _templates;
		private ILogger<TemplatesController> _logger;

		private TemplatesController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("templates-controller").Options);
			_templates = new Mock<ITemplateRepository>();
			_logger = new TestLogger<TemplatesController>();

			_uut = new TemplatesController(_context, _templates.Object, _logger);
		}

		[TearDown]
		public void TearDown() => _context.Database.EnsureDeleted();

		[Test]
		public async Task GetGets()
		{
			var template = new Template
			{
				ReportType = ReportType.MonthlyInvoice,
				SchoolYear = "2017-2018",
				Name = "template",
				Content = Encoding.UTF8.GetBytes("hello"),
			};
			_templates.Setup(ts => ts.Get(template.ReportType, template.SchoolYear)).Returns(template);

			var result = await _uut.Get(template.ReportType.Value, template.SchoolYear);
			Assert.That(result, Is.TypeOf<FileStreamResult>());
			var file = (FileStreamResult)result;

			Assert.That(file.FileDownloadName, Is.EqualTo(template.Name));
			Assert.That(file.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

			var stream = file.FileStream;
			using (var reader = new StreamReader(stream))
				Assert.That(reader.ReadToEnd(), Is.EqualTo(Encoding.UTF8.GetString(template.Content)));
		}

		[Test]
		public async Task GetReturnsNotFound()
		{
			var type = ReportType.MonthlyInvoice;
			var year = "2017-2018";
			_templates.Setup(ts => ts.Get(type, year)).Returns<Template>(null);

			var result = await _uut.Get(type.Value, year);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task GetReturnsBadRequestWhenInvalidReportType()
		{
			var type = "bob";
			var year = "2017-2018";

			var result = await _uut.Get(type, year);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var err = ((ErrorResponse)value).Error;

			Assert.That(err, Is.EqualTo("Invalid ReportType 'bob'."));
		}

		[Test]
		public async Task UploadUploads()
		{
			var formFile = new Mock<IFormFile>();

			var filepath = "../../../TestData/test-template.xlsx";
			var file = File.OpenRead(filepath);
			var name = "template";
			formFile.Setup(ff => ff.FileName).Returns(name);
			formFile.Setup(ff => ff.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			formFile.Setup(ff => ff.OpenReadStream()).Returns(file);

			var type = ReportType.MonthlyInvoice;
			var year = "2017-2018";

			byte[] content;
			using (var ms = new MemoryStream())
			{
				File.OpenRead(filepath).CopyTo(ms);
				content = ms.ToArray();
			}

			_templates.Setup(ts => ts.CreateOrUpdate(It.Is<Template>(t =>
				t.ReportType == type &&
				t.SchoolYear == year &&
				t.Name == name &&
				t.Content.SequenceEqual(content)
			))).Returns<Template>(t => t).Verifiable();

			var result = await _uut.Upload(type.Value, year, formFile.Object);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			Assert.That(((CreatedResult)result).Location, Is.EqualTo($"/api/templates/{type}/{year}"));
			var value = ((CreatedResult)result).Value;

			Assert.That(value, Is.TypeOf<TemplatesController.TemplateResponse>());
			var actual = ((TemplatesController.TemplateResponse)value).Template;

			Assert.That(actual.ReportType, Is.EqualTo(type));
			Assert.That(actual.SchoolYear, Is.EqualTo(year));
			Assert.That(actual.Name, Is.EqualTo(name));

			_templates.Verify();
		}

		[Test]
		public async Task UploadReturnsBadRequestWhenInvalidContentType()
		{
			var formFile = new Mock<IFormFile>();
			var contentType = "bad";
			formFile.Setup(f => f.ContentType).Returns(contentType);

			var result = await _uut.Upload(ReportType.MonthlyInvoice.Value, "2017-2018", formFile.Object);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var actual = ((ErrorResponse)value).Error;

			Assert.That(actual, Is.EqualTo($"Invalid file Content-Type '{contentType}'."));
		}

		[Test]
		public async Task UploadReturnsBadRequestWhenInvalidReportType()
		{
			var result = await _uut.Upload("bob", "2017-2018", null);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var err = ((ErrorResponse)value).Error;

			Assert.That(err, Is.EqualTo("Invalid ReportType 'bob'."));
		}
	}
}
