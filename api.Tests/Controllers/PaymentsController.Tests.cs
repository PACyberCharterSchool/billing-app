using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		private Mock<IPaymentRepository> _payments;
		private ILogger<PaymentsController> _logger;

		private PaymentsController _uut;

		[SetUp]
		public void SetUp()
		{
			_payments = new Mock<IPaymentRepository>();
			_logger = new TestLogger<PaymentsController>();

			_uut = new PaymentsController(_payments.Object, _logger);
		}

		[Test]
		public void CreateCreates()
		{
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
				new Payment { PaymentId = id, Split = 1, Amount = 10m },
				new Payment { PaymentId = id, Split = 2, Amount = 20m },
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
	}
}
