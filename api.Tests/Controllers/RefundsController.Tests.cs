using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
	public class RefundsControllerTests
	{
		private PacBillContext _context;
		private Mock<IRefundRepository> _refunds;
		private Mock<ISchoolDistrictRepository> _districts;
		private ILogger<RefundsController> _logger;

		private RefundsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("refunds-controller").Options);
			_refunds = new Mock<IRefundRepository>();
			_districts = new Mock<ISchoolDistrictRepository>();
			_logger = new TestLogger<RefundsController>();

			_uut = new RefundsController(_context, _refunds.Object, _districts.Object, _logger);
		}

		[Test]
		public async Task CreateCreates()
		{
			var create = new RefundsController.CreateUpdateRefund
			{
				Amount = 10m,
				CheckNumber = "1234",
				Date = DateTime.Now.Date,
				SchoolYear = "2017-2018",
				SchoolDistrictAun = 123456789,
			};
			_refunds.Setup(rs => rs.Create(It.IsAny<Refund>())).Returns<Refund>(r => r);
			_districts.Setup(ds => ds.GetByAun(create.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = create.SchoolDistrictAun });

			var username = "bob";
			_uut.SetUsername(username);

			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			Assert.That(((CreatedResult)result).Location, Is.SupersetOf("/api/refunds"));
			var value = ((CreatedResult)result).Value;

			Assert.That(value, Is.TypeOf<RefundsController.RefundResponse>());
			var actual = ((RefundsController.RefundResponse)value).Refund;

			Assert.That(actual.Amount, Is.EqualTo(create.Amount));
			Assert.That(actual.CheckNumber, Is.EqualTo(create.CheckNumber));
			Assert.That(actual.Date, Is.EqualTo(create.Date));
			Assert.That(actual.Username, Is.EqualTo(username));
			Assert.That(actual.SchoolYear, Is.EqualTo(create.SchoolYear));
			Assert.That(actual.SchoolDistrict, Is.Not.Null);
			Assert.That(actual.SchoolDistrict.Aun, Is.EqualTo(create.SchoolDistrictAun));

			_refunds.Verify(rs => rs.Create(It.Is<Refund>(r =>
				r.Amount == create.Amount &&
				r.CheckNumber == create.CheckNumber &&
				r.Date == create.Date &&
				r.Username == username &&
				r.SchoolYear == create.SchoolYear &&
				(r.SchoolDistrict != null && r.SchoolDistrict.Aun == create.SchoolDistrictAun)
			)), Times.Once);
		}

		[Test]
		public async Task CreateReturnsBadRequest()
		{
			var key = "err";
			var msg = "msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.Create(new RefundsController.CreateUpdateRefund());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actual = ((ErrorsResponse)value).Errors;
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0], Is.EqualTo(msg));
		}

		public async Task CreateReturnsConflict()
		{
			_refunds.Setup(rs => rs.Create(It.IsAny<Refund>())).Throws(new DbUpdateException("", new Exception()));

			var create = new RefundsController.CreateUpdateRefund
			{
				Amount = 10m,
				CheckNumber = "1234",
				Date = DateTime.Now.Date,
				SchoolYear = "2017-2018",
				SchoolDistrictAun = 123456789,
			};
			_districts.Setup(ds => ds.GetByAun(create.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = create.SchoolDistrictAun });

			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			var code = ((StatusCodeResult)result).StatusCode;

			Assert.That(code, Is.EqualTo(409));
		}

		private static Refund NewRefund(DateTime time, int id) => new Refund
		{
			Id = id,
			Amount = 10m,
			CheckNumber = "1234",
			Date = time.Date,
			SchoolYear = "2017-2018",
			Username = "bob",
			Created = time,
			LastUpdated = time,
			SchoolDistrict = new SchoolDistrict
			{
				Aun = 123456789,
			},
		};

		private static void AssertRefund(RefundDto actual, Refund refund)
		{
			Assert.That(actual.Id, Is.EqualTo(refund.Id));
			Assert.That(actual.Amount, Is.EqualTo(refund.Amount));
			Assert.That(actual.CheckNumber, Is.EqualTo(refund.CheckNumber));
			Assert.That(actual.Date, Is.EqualTo(refund.Date));
			Assert.That(actual.SchoolYear, Is.EqualTo(refund.SchoolYear));
			Assert.That(actual.Username, Is.EqualTo(refund.Username));
			Assert.That(actual.Created, Is.EqualTo(refund.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(refund.LastUpdated));
			Assert.That(actual.SchoolDistrict.Aun, Is.EqualTo(refund.SchoolDistrict.Aun));
		}

		[Test]
		public async Task GetGets()
		{
			var time = DateTime.Now;
			var refund = NewRefund(time, 3);
			_refunds.Setup(rs => rs.Get(refund.Id)).Returns(refund);

			var result = await _uut.Get(refund.Id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<RefundsController.RefundResponse>());
			var actual = ((RefundsController.RefundResponse)value).Refund;
			AssertRefund(actual, refund);
		}

		[Test]
		public async Task GetReturnsNotFound()
		{
			var id = 3;
			_refunds.Setup(rs => rs.Get(id)).Throws(new NotFoundException());

			var result = await _uut.Get(id);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task GetManyGetsMany()
		{
			var time = DateTime.Now;
			var refunds = new[] {
				NewRefund(time, 1),
				NewRefund(time, 2),
			};
			_refunds.Setup(rs => rs.GetMany()).Returns(refunds);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<RefundsController.RefundsResponse>());
			var actuals = ((RefundsController.RefundsResponse)value).Refunds;
			Assert.That(actuals, Has.Count.EqualTo(refunds.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertRefund(actuals[i], refunds[i]);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_refunds.Setup(rs => rs.GetMany()).Returns((IList<Refund>)null);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<RefundsController.RefundsResponse>());
			var actuals = ((RefundsController.RefundsResponse)value).Refunds;
			Assert.That(actuals, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_refunds.Setup(rs => rs.GetMany()).Returns(new List<Refund>());

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<RefundsController.RefundsResponse>());
			var actuals = ((RefundsController.RefundsResponse)value).Refunds;
			Assert.That(actuals, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task UpdateUpdates()
		{
			var id = 3;
			var update = new RefundsController.CreateUpdateRefund
			{
				Amount = 10m,
				CheckNumber = "1234",
				Date = DateTime.Now.Date,
				SchoolYear = "2017-2018",
				SchoolDistrictAun = 123456789,
			};

			var username = "bob";
			_uut.SetUsername(username);

			_refunds.Setup(rs => rs.Update(It.Is<Refund>(r =>
				r.Amount == update.Amount &&
				r.CheckNumber == update.CheckNumber &&
				r.Date == update.Date &&
				r.Username == username &&
				r.SchoolYear == update.SchoolYear &&
				(r.SchoolDistrict != null && r.SchoolDistrict.Aun == update.SchoolDistrictAun)
			))).Returns<Refund>(r => r).Verifiable();
			_districts.Setup(ds => ds.GetByAun(update.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = update.SchoolDistrictAun });

			var result = await _uut.Update(id, update);
			Assert.That(result, Is.TypeOf<OkResult>());

			_refunds.Verify();
		}

		[Test]
		public async Task UpdateReturnsBadRequest()
		{
			var key = "err";
			var msg = "msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.Update(3, new RefundsController.CreateUpdateRefund());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;
			Assert.That(actuals[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task UpdateReturnsNotFound()
		{
			_refunds.Setup(rs => rs.Update(It.IsAny<Refund>())).Throws(new NotFoundException());

			var id = 3;
			var update = new RefundsController.CreateUpdateRefund
			{
				Amount = 10m,
				CheckNumber = "1234",
				Date = DateTime.Now.Date,
				SchoolYear = "2017-2018",
				SchoolDistrictAun = 123456789,
			};
			_districts.Setup(ds => ds.GetByAun(update.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = update.SchoolDistrictAun });

			_uut.SetUsername("bob");

			var result = await _uut.Update(id, update);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}
	}
}
