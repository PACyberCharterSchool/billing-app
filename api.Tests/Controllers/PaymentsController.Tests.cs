using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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
	public class PaymentsControllerTests
	{
		private PacBillContext _context;
		private Mock<IPaymentRepository> _payments;
		private Mock<ISchoolDistrictRepository> _districts;
		private ILogger<PaymentsController> _logger;

		private PaymentsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("payments-controller").Options);
			_payments = new Mock<IPaymentRepository>();
			_districts = new Mock<ISchoolDistrictRepository>();
			_logger = new TestLogger<PaymentsController>();

			_uut = new PaymentsController(_context, _payments.Object, _districts.Object, _logger);
		}

		[Test]
		public async Task CreateCreates()
		{
			var create = new PaymentsController.CreateUpdatePayment
			{
				ExternalId = "bob",
				Type = PaymentType.Check,
				SchoolDistrictAun = 123456789,
				Splits = new[]
				{
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 10m,
						SchoolYear = "2017-2018",
						Date = DateTime.Now
					},
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 20m,
						SchoolYear = "2018-2019",
						Date = DateTime.Now
					},
				}
			};
			_payments.Setup(ps => ps.CreateMany(It.IsAny<IList<Payment>>())).Returns<IList<Payment>>(l => l);

			_districts.Setup(ds => ds.GetByAun(create.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = create.SchoolDistrictAun });

			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			var location = ((CreatedResult)result).Location;

			Assert.That(location, Is.SupersetOf("/api/payments/"));

			_payments.Verify(ps => ps.CreateMany(It.Is<IList<Payment>>(l =>
				l.Count == 2 &&
					l[0].Date == create.Splits[0].Date &&
					l[0].ExternalId == create.ExternalId &&
					l[0].Type == create.Type &&
					l[0].SchoolDistrict.Aun == create.SchoolDistrictAun &&
					l[0].Split == 1 &&
					l[0].Amount == create.Splits[0].Amount &&
					l[0].SchoolYear == create.Splits[0].SchoolYear &&

					l[1].Date == create.Splits[1].Date &&
					l[1].ExternalId == create.ExternalId &&
					l[1].Type == create.Type &&
					l[1].SchoolDistrict.Aun == create.SchoolDistrictAun &&
					l[1].Split == 2 &&
					l[1].Amount == create.Splits[1].Amount &&
					l[1].SchoolYear == create.Splits[1].SchoolYear
			)), Times.Once);
		}

		[Test]
		public async Task CreateReturnsBadRequest()
		{
			var key = "key";
			var msg = "err";
			_uut.ModelState.AddModelError(key, msg);

			var create = new PaymentsController.CreateUpdatePayment();
			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actual = ((ErrorsResponse)value).Errors;

			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task CreateReturnsConflict()
		{
			var create = new PaymentsController.CreateUpdatePayment
			{
				ExternalId = "bob",
				Type = PaymentType.Check,
				SchoolDistrictAun = 123456789,
				Splits = new[]
				{
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 10m,
						SchoolYear = "2017-2018",
						Date = DateTime.Now
					},
				}
			};

			_payments.Setup(ps => ps.CreateMany(It.IsAny<IList<Payment>>())).
				Throws(new DbUpdateException("", new Exception()));

			var result = await _uut.Create(create);
			Assert.That(result, Is.TypeOf<StatusCodeResult>());
			var code = ((StatusCodeResult)result).StatusCode;

			Assert.That(code, Is.EqualTo(409));
		}

		public void AssertPayment(PaymentDto actual, Payment payment)
		{
			Assert.That(actual.Id, Is.EqualTo(payment.Id));
			Assert.That(actual.PaymentId, Is.EqualTo(payment.PaymentId));
			Assert.That(actual.Split, Is.EqualTo(payment.Split));
			Assert.That(actual.Date, Is.EqualTo(payment.Date));
			Assert.That(actual.ExternalId, Is.EqualTo(payment.ExternalId));
			Assert.That(actual.Type, Is.EqualTo(payment.Type));
			Assert.That(actual.Amount, Is.EqualTo(payment.Amount));
			Assert.That(actual.SchoolYear, Is.EqualTo(payment.SchoolYear));
			Assert.That(actual.Created, Is.EqualTo(payment.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(payment.LastUpdated));
		}

		[Test]
		public async Task GetManyByIdReturnsPayments()
		{
			var id = "1234";
			var payments = new[] {
				new Payment { PaymentId = id, Split = 1, Amount = 10m, SchoolDistrict = new SchoolDistrict() },
				new Payment { PaymentId = id, Split = 2, Amount = 20m, SchoolDistrict = new SchoolDistrict() },
			};
			_payments.Setup(ps => ps.GetMany(id)).Returns(payments);

			var result = await _uut.GetManyById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertPayment(actuals[i], payments[i]);
		}

		[Test]
		public async Task GetManyByIdReturnsEmptyListWhenEmpty()
		{
			var id = "1234";
			_payments.Setup(ps => ps.GetMany(id)).Returns(new List<Payment>());

			var result = await _uut.GetManyById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public async Task GetManyByIdReturnsEmptyListWhenNull()
		{
			var id = "1234";
			_payments.Setup(ps => ps.GetMany(id)).Returns((List<Payment>)null);

			var result = await _uut.GetManyById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public async Task GetManyReturnsPayments()
		{
			var id = "1234";
			var payments = new[] {
				new Payment { PaymentId = id, Split = 1, Amount = 10m, SchoolDistrict = new SchoolDistrict() },
				new Payment { PaymentId = id, Split = 2, Amount = 20m, SchoolDistrict = new SchoolDistrict() },
			};
			_payments.Setup(ps => ps.GetMany()).Returns(payments);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actuals.Count; i++)
				AssertPayment(actuals[i], payments[i]);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_payments.Setup(ps => ps.GetMany()).Returns(new List<Payment>());

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_payments.Setup(ps => ps.GetMany()).Returns((List<Payment>)null);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<PaymentsController.PaymentsResponse>());
			var actuals = ((PaymentsController.PaymentsResponse)value).Payments;

			Assert.That(actuals, Is.Empty);
		}


		[Test]
		public async Task UpdateUpdates()
		{
			var update = new PaymentsController.CreateUpdatePayment
			{
				ExternalId = "bob",
				Type = PaymentType.Check,
				SchoolDistrictAun = 123456789,
				Splits = new[]
				{
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 10m,
						SchoolYear = "2017-2018",
						Date = DateTime.Now
					},
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 20m,
						SchoolYear = "2018-2019",
						Date = DateTime.Now
					},
				}
			};
			_payments.Setup(ps => ps.UpdateMany(It.IsAny<IList<Payment>>())).Returns<IList<Payment>>(l => l);

			_districts.Setup(ds => ds.GetByAun(update.SchoolDistrictAun)).
				Returns(new SchoolDistrict { Aun = update.SchoolDistrictAun });

			var paymentId = "1234";
			var result = await _uut.Update(paymentId, update);
			Assert.That(result, Is.TypeOf<OkResult>());

			_payments.Verify(ps => ps.UpdateMany(It.Is<IList<Payment>>(l =>
				l.Count == 2 &&
					l[0].PaymentId == paymentId &&
					l[0].Date == update.Splits[0].Date &&
					l[0].ExternalId == update.ExternalId &&
					l[0].Type == update.Type &&
					l[0].SchoolDistrict.Aun == update.SchoolDistrictAun &&
					l[0].Split == 1 &&
					l[0].Amount == update.Splits[0].Amount &&
					l[0].SchoolYear == update.Splits[0].SchoolYear &&

					l[1].PaymentId == paymentId &&
					l[1].Date == update.Splits[1].Date &&
					l[1].ExternalId == update.ExternalId &&
					l[1].Type == update.Type &&
					l[1].SchoolDistrict.Aun == update.SchoolDistrictAun &&
					l[1].Split == 2 &&
					l[1].Amount == update.Splits[1].Amount &&
					l[1].SchoolYear == update.Splits[1].SchoolYear
			)), Times.Once);
		}

		[Test]
		public async Task UpdateReturnsBadRequest()
		{
			var key = "err";
			var msg = "msg";
			_uut.ModelState.AddModelError(key, msg);

			var update = new PaymentsController.CreateUpdatePayment();
			var result = await _uut.Update("1234", update);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var actuals = ((ErrorsResponse)value).Errors;

			Assert.That(actuals, Has.Count.EqualTo(1));
			Assert.That(actuals[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task UpdateReturnsNotFound()
		{
			var update = new PaymentsController.CreateUpdatePayment
			{
				ExternalId = "bob",
				Type = PaymentType.Check,
				SchoolDistrictAun = 123456789,
				Splits = new[]
				{
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 10m,
						SchoolYear = "2017-2018",
						Date = DateTime.Now
					},
					new PaymentsController.CreateUpdatePayment.Split
					{
						Amount = 20m,
						SchoolYear = "2018-2019",
						Date = DateTime.Now
					},
				}
			};
			_payments.Setup(ps => ps.UpdateMany(It.IsAny<IList<Payment>>())).Throws(new NotFoundException());

			var result = await _uut.Update("1234", update);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}
	}
}
