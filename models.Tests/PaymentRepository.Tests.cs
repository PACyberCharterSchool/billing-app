using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using NUnit.Framework;

using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class PaymentRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<PaymentRepository> _logger;

		private PaymentRepository _uut;

		private PacBillContext NewContext()
		{
			return new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("payments").Options);
		}

		[SetUp]
		public void SetUp()
		{
			_context = NewContext();
			_logger = new TestLogger<PaymentRepository>();

			_uut = new PaymentRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void CreateOrUpdateManyWithNewCreates()
		{
			var paymentId = "1234";
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1},
				new Payment { PaymentId = paymentId, Split = 2},
			};

			var time = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdateMany(time, payments));
			Assert.That(result, Is.EqualTo(payments));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count; i++)
			{
				Assert.That(actual[i], Is.EqualTo(payments[i]));
				Assert.That(actual[i].Created, Is.EqualTo(time));
				Assert.That(actual[i].LastUpdated, Is.EqualTo(time));
			}
		}

		[Test]
		public void CreateOrUpdateManyWithExistingUpdates()
		{
			var paymentId = "1234";
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1 },
				new Payment { PaymentId = paymentId, Split = 2 },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var amount = 1.0m;
			payments[0].Amount = amount;
			payments[1].Amount = amount;

			var time = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdateMany(payments));
			Assert.That(result, Has.Count.EqualTo(payments.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count(); i++)
				Assert.That(actual[i].Amount, Is.EqualTo(amount));
		}

		[Test]
		public void CreateOrUpdateManyWithDifferentObjectsUpdates()
		{
			var paymentId = "1234";
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1 },
				new Payment { PaymentId = paymentId, Split = 2 },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var amount = 1.0m;
			var updates = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = amount },
				new Payment { PaymentId = paymentId, Split = 2, Amount = amount },
			};

			var time = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateOrUpdateMany(updates));
			Assert.That(result, Has.Count.EqualTo(payments.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count(); i++)
				Assert.That(actual[i].Amount, Is.EqualTo(amount));
		}

		[Test]
		public void GetManyReturnsListOrderBySplit()
		{
			var paymentId = "1234";
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1 },
				new Payment { PaymentId = paymentId, Split = 3 },
				new Payment { PaymentId = paymentId, Split = 2 },
			};
			_context.AddRange(payments);
			_context.SaveChanges();

			var actual = _uut.GetMany(paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			Assert.That(actual[0].Split, Is.EqualTo(payments[0].Split));
			Assert.That(actual[1].Split, Is.EqualTo(payments[2].Split));
			Assert.That(actual[2].Split, Is.EqualTo(payments[1].Split));
		}
	}
}
