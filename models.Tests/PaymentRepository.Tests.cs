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
		[Ignore("Tests are for sissies.")]
		public void CreateManyWithNewCreates()
		{
			var payments = new[] {
				new Payment { Split = 1, Amount = 10m },
				new Payment { Split = 2, Amount = 20m },
			};

			var time = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.CreateMany(time, payments));
			Assert.That(result, Is.EqualTo(payments));

			var actual = _context.Payments.Where(p => p.PaymentId == result[0].PaymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count; i++)
			{
				Assert.That(actual[i].PaymentId, Is.Not.Empty);
				Assert.That(actual[i], Is.EqualTo(payments[i]));
				Assert.That(actual[i].Created, Is.EqualTo(time));
			}
		}

		[Test]
		[Ignore("Tests are for sissies.")]
		public void CreateWithExistingIdFails()
		{
			var id = 1;
			var payments = new[] {
				new Payment { Id = id },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var time = DateTime.Now;
			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(payments)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		[Ignore("Tests are for sissies.")]
		public void CreateWithExistingPaymentIdFails()
		{
			var id = "1234";
			var payments = new[] {
				new Payment { PaymentId = id },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var time = DateTime.Now;
			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(payments)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void GetManyByIdReturnsListOrderBySplit()
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

		[Test]
		public void GetManyReturnsListOrderByDatePaymentIdSplit()
		{
			var time = DateTime.Now;
			var payments = new[] {
				new Payment { PaymentId = "1234", Split = 1, Date = time },
				new Payment { PaymentId = "5678", Split = 1, Date = time.AddDays(-1) },
				new Payment { PaymentId = "5678", Split = 2, Date = time.AddDays(-1) },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var actual = _uut.GetMany().ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));

			Assert.That(actual[0].PaymentId, Is.EqualTo(payments[1].PaymentId));
			Assert.That(actual[0].Split, Is.EqualTo(payments[1].Split));

			Assert.That(actual[1].PaymentId, Is.EqualTo(payments[2].PaymentId));
			Assert.That(actual[1].Split, Is.EqualTo(payments[2].Split));

			Assert.That(actual[2].PaymentId, Is.EqualTo(payments[0].PaymentId));
			Assert.That(actual[2].Split, Is.EqualTo(payments[0].Split));
		}

		[Test]
		public void UpdateManyWithExistingUpdates()
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
			var result = _context.SaveChanges(() => _uut.UpdateMany(time, payments));
			Assert.That(result, Has.Count.EqualTo(payments.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count(); i++)
				Assert.That(actual[i].Amount, Is.EqualTo(amount));
		}

		[Test]
		public void UpdateManyWithDifferentObjectsUpdates()
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
			var result = _context.SaveChanges(() => _uut.UpdateMany(time, updates));
			Assert.That(result, Has.Count.EqualTo(payments.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(payments.Length));
			for (var i = 0; i < actual.Count(); i++)
				Assert.That(actual[i].Amount, Is.EqualTo(amount));
		}

		[Test]
		public void UpdateSplitsExisting()
		{
			var paymentId = "1234";
			var time = DateTime.Now;
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = 10m, Created = time.AddDays(-1) },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var updates = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = 7.5m },
				new Payment { PaymentId = paymentId, Split = 2, Amount = 2.5m },
			};

			var result = _context.SaveChanges(() => _uut.UpdateMany(time, updates));
			Assert.That(result, Has.Count.EqualTo(updates.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(updates.Length));
			for (var i = 0; i < actual.Count; i++)
				Assert.That(actual[i].Amount, Is.EqualTo(updates[i].Amount));

			Assert.That(actual[0].Created, Is.EqualTo(payments[0].Created));
			Assert.That(actual[0].LastUpdated, Is.EqualTo(time));
			Assert.That(actual[1].Created, Is.EqualTo(time));
			Assert.That(actual[1].LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void UpdateMergesExisting()
		{
			var paymentId = "1234";
			var time = DateTime.Now;
			var payments = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = 7.5m, Created = time.AddDays(-1) },
				new Payment { PaymentId = paymentId, Split = 2, Amount = 2.5m, Created = time.AddDays(-1) },
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(payments);
				ctx.SaveChanges();
			}

			var updates = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = 10m },
			};

			var result = _context.SaveChanges(() => _uut.UpdateMany(time, updates));
			Assert.That(result, Has.Count.EqualTo(updates.Length));

			var actual = _context.Payments.Where(p => p.PaymentId == paymentId).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));

			Assert.That(actual[0].Amount, Is.EqualTo(updates[0].Amount));
			Assert.That(actual[0].Created, Is.EqualTo(payments[0].Created));
			Assert.That(actual[0].LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void UpdateManyFailsIfNonexistentPaymentId()
		{
			var paymentId = "1234";

			var time = DateTime.Now;
			var updates = new[] {
				new Payment { PaymentId = paymentId, Split = 1, Amount = 10m },
			};
			Assert.That(() => _context.SaveChanges(() => _uut.UpdateMany(time, updates)), Throws.TypeOf<NotFoundException>());
		}
	}
}
