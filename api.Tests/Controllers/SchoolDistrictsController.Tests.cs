using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Dtos;
using api.Tests.Util;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class SchoolDistrictsControllerTests
	{
		private PacBillContext _context;
		private Mock<ISchoolDistrictRepository> _schoolDistricts;
		private ILogger<SchoolDistrictsController> _logger;

		private SchoolDistrictsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("schools-district-controller").Options);
			_schoolDistricts = new Mock<ISchoolDistrictRepository>();
			_logger = new TestLogger<SchoolDistrictsController>();

			_uut = new SchoolDistrictsController(_context, _schoolDistricts.Object, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		public void AssertSchoolDistrict(SchoolDistrictDto actual, SchoolDistrict district)
		{
			Assert.That(actual.Id, Is.EqualTo(district.Id));
			Assert.That(actual.Aun, Is.EqualTo(district.Aun));
			Assert.That(actual.Name, Is.EqualTo(district.Name));
			Assert.That(actual.Rate, Is.EqualTo(district.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(district.AlternateRate));
			Assert.That(actual.PaymentType, Is.EqualTo(district.PaymentType));
			Assert.That(actual.Created, Is.EqualTo(district.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(district.LastUpdated));
		}

		[Test]
		public async Task GetByIdReturnsSchoolDistrict()
		{
			var id = 3;
			var district = new SchoolDistrict { Id = 3 };
			_schoolDistricts.Setup(d => d.Get(id)).Returns(district);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictResponse>());
			var actual = ((SchoolDistrictsController.SchoolDistrictResponse)value).SchoolDistrict;

			AssertSchoolDistrict(actual, district);
		}

		[Test]
		public async Task GetByIdReturnsNotFound()
		{
			var id = 3;
			_schoolDistricts.Setup(d => d.Get(id)).Returns((SchoolDistrict)null);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task GetManyReturnsAll()
		{
			var districts = new[] {
				new SchoolDistrict{Id = 1},
				new SchoolDistrict{Id = 2},
				new SchoolDistrict{Id = 3},
			};
			_schoolDistricts.Setup(d => d.GetMany()).Returns(districts);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());
			var actuals = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;

			Assert.That(actuals, Has.Count.EqualTo(districts.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertSchoolDistrict(actuals[i], districts[i]);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_schoolDistricts.Setup(d => d.GetMany()).Returns((IList<SchoolDistrict>)null);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());
			var actuals = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;

			Assert.That(actuals, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_schoolDistricts.Setup(d => d.GetMany()).Returns(new List<SchoolDistrict>());

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());
			var actuals = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;

			Assert.That(actuals, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task UpdateUpdates()
		{
			var id = 3;
			_schoolDistricts.Setup(d => d.Get(id)).Returns(new SchoolDistrict());

			var result = await _uut.Update(id, new SchoolDistrictsController.SchoolDistrictUpdate());
			Assert.That(result, Is.TypeOf<OkResult>());
			_schoolDistricts.Verify(d => d.CreateOrUpdate(It.IsAny<SchoolDistrict>()), Times.Once);
		}

		[Test]
		public async Task UpdateReturnsBadRequest()
		{
			var key = "err";
			var msg = "msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.Update(3, new SchoolDistrictsController.SchoolDistrictUpdate());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

			var value = ((BadRequestObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<ErrorsResponse>());

			var actual = ((ErrorsResponse)value).Errors;
			Assert.That(actual[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task UpdateReturnsNotFound()
		{
			var id = 3;
			_schoolDistricts.Setup(d => d.Get(id)).Returns((SchoolDistrict)null);

			var result = await _uut.Update(id, new SchoolDistrictsController.SchoolDistrictUpdate());
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}
	}
}
