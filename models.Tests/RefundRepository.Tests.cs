using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using NUnit.Framework;

using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class RefundRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<RefundRepository> _logger;

		private RefundRepository _uut;

		private static PacBillContext NewContext()
		{
			return new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("refund-repository").Options);
		}

		[SetUp]
		public void SetUp()
		{
			_context = NewContext();
			_logger = new TestLogger<RefundRepository>();

			_uut = new RefundRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown() => _context.Database.EnsureDeleted();

		public static void AssertRefund(Refund actual, Refund refund)
		{
			Assert.That(actual.Amount, Is.EqualTo(refund.Amount));
			Assert.That(actual.Date, Is.EqualTo(refund.Date));
			Assert.That(actual.Username, Is.EqualTo(refund.Username));
		}

		[Test]
		public void CreateCreates()
		{
			var time = DateTime.Now;
			var refund = new Refund
			{
				Amount = 10m,
				Date = time.Date,
				Username = "bob",
			};

			var result = _context.SaveChanges(() => _uut.Create(time, refund));
			Assert.That(result, Is.EqualTo(refund));

			var actual = NewContext().Refunds.Single(r => r.Id == result.Id);
			AssertRefund(actual, refund);
			Assert.That(actual.Created, Is.EqualTo(time));
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateThrowsOnConflict()
		{
			var refund = new Refund();
			using (var ctx = NewContext())
			{
				ctx.Add(refund);
				ctx.SaveChanges();
			}

			Assert.That(() => _context.SaveChanges(() => _uut.Create(refund)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void CreateManyCreates()
		{
			var time = DateTime.Now;
			var refunds = new[] {
				new Refund{
					Amount = 10m,
					Date = time,
					Username = "alice",
				},
				new Refund{
					Amount = 20m,
					Date = time,
					Username = "bob",
				},
			};

			var results = _context.SaveChanges(() => _uut.CreateMany(time, refunds));
			Assert.That(results, Has.Length.EqualTo(refunds.Length));

			var actuals = NewContext().Refunds.ToList();
			for (var i = 0; i < actuals.Count; i++)
			{
				AssertRefund(actuals[i], refunds[i]);
				Assert.That(actuals[i].Created, Is.EqualTo(time));
				Assert.That(actuals[i].LastUpdated, Is.EqualTo(time));
			}
		}

		[Test]
		public void CreateManyThrowsOnConflict()
		{
			var refunds = new[] {
				new Refund(),
				new Refund(),
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(refunds);
				ctx.SaveChanges();
			}

			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(refunds)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void GetGets()
		{
			var refund = new Refund();
			using (var ctx = NewContext())
			{
				ctx.Add(refund);
				ctx.SaveChanges();
			}

			var actual = _uut.Get(refund.Id);
			AssertRefund(actual, refund);
		}

		[Test]
		public void GetReturnsNullWhenNotFound()
		{
			var actual = _uut.Get(1);
			Assert.IsNull(actual);
		}

		[Test]
		public void GetManyGetsMany()
		{
			var time = DateTime.Now;
			var refunds = new[] {
				new Refund{
					Amount = 10m,
					Date = time,
				},
				new Refund{
					Amount = 20m,
					Date = time.AddDays(-1),
				},
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(refunds);
				ctx.SaveChanges();
			}

			var actuals = _uut.GetMany().ToList();
			Assert.That(actuals, Has.Count.EqualTo(refunds.Length));
			AssertRefund(actuals[0], refunds[1]);
			AssertRefund(actuals[1], refunds[0]);
		}

		[Test]
		public void GetManyReturnsEmptyListWhenNotFound()
		{
			var actuals = _uut.GetMany().ToList();
			Assert.That(actuals, Is.Empty);
		}

		[Test]
		public void UpdateUpdates()
		{
			var time = DateTime.Now;
			var refund = new Refund
			{
				Amount = 10m,
				Date = time.Date,
				Username = "bob",
				Created = time.AddDays(-1),
				LastUpdated = time.AddDays(-1),
			};
			using (var ctx = NewContext())
			{
				ctx.Add(refund);
				ctx.SaveChanges();
			}

			refund.Amount = 20m;

			var result = _context.SaveChanges(() => _uut.Update(time, refund));
			AssertRefund(result, refund);

			var actual = NewContext().Refunds.Single(r => r.Id == result.Id);
			AssertRefund(actual, refund);
			Assert.That(actual.Created, Is.EqualTo(time.AddDays(-1)));
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void UpdateUpdatesDifferentObject()
		{
			var time = DateTime.Now;
			var refund = new Refund
			{
				Amount = 10m,
				Date = time.Date,
				Username = "bob",
				Created = time.AddDays(-1),
				LastUpdated = time.AddDays(-1),
			};
			using (var ctx = NewContext())
			{
				ctx.Add(refund);
				ctx.SaveChanges();
			}

			var update = new Refund
			{
				Id = refund.Id,
				Amount = 20m,
				Date = refund.Date,
				Username = "bob",
				Created = refund.Created,
				LastUpdated = refund.LastUpdated,
			};

			var result = _context.SaveChanges(() => _uut.Update(time, update));
			AssertRefund(result, update);

			var actual = NewContext().Refunds.Single(r => r.Id == refund.Id);
			AssertRefund(actual, update);
		}

		[Test]
		public void UpdateReturnsNotFound()
		{
			var refund = new Refund { Id = 1 };
			Assert.That(() => _context.SaveChanges(() => _uut.Update(refund)), Throws.TypeOf<NotFoundException>());
		}

		[Test]
		public void UpdateManyUpdatesMany()
		{
			var time = DateTime.Now;
			var refunds = new[] {
				new Refund{
					Amount = 10m,
					Date = time.Date,
					Username = "alice",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
				new Refund{
					Amount = 20m,
					Date = time.Date,
					Username = "bob",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(refunds);
				ctx.SaveChanges();
			}

			foreach (var refund in refunds)
				refund.Amount = 30m;

			var results = _context.SaveChanges(() => _uut.UpdateMany(time, refunds));
			Assert.That(results, Has.Count.EqualTo(refunds.Length));
			for (var i = 0; i < results.Count; i++)
				AssertRefund(results[i], refunds[i]);

			var actuals = NewContext().Refunds.ToList();
			Assert.That(actuals, Has.Count.EqualTo(refunds.Length));
			for (var i = 0; i < actuals.Count; i++)
			{
				AssertRefund(actuals[i], refunds[i]);
				Assert.That(actuals[i].LastUpdated, Is.EqualTo(time));
			}
		}

		[Test]
		public void UpdateManyUpdatesManyDifferentObjects()
		{
			var time = DateTime.Now;
			var refunds = new[] {
				new Refund{
					Amount = 10m,
					Date = time.Date,
					Username = "alice",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
				new Refund{
					Amount = 20m,
					Date = time.Date,
					Username = "bob",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
			};
			using (var ctx = NewContext())
			{
				ctx.AddRange(refunds);
				ctx.SaveChanges();
			}

			var updates = new[] {
				new Refund{
					Id = refunds[0].Id,
					Amount = 30m,
					Date = time.Date,
					Username = "alice",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
				new Refund{
					Id = refunds[1].Id,
					Amount = 40m,
					Date = time.Date,
					Username = "bob",
					Created = time.AddDays(-1),
					LastUpdated = time.AddDays(-1),
				},
			};

			var results = _context.SaveChanges(() => _uut.UpdateMany(time, updates));
			Assert.That(results.Count, Is.EqualTo(refunds.Length));
			for (var i = 0; i < results.Count; i++)
				AssertRefund(results[i], updates[i]);

			var actuals = NewContext().Refunds.ToList();
			Assert.That(actuals, Has.Count.EqualTo(updates.Length));
			for (var i = 0; i < actuals.Count; i++)
			{
				AssertRefund(actuals[i], updates[i]);
				Assert.That(actuals[i].LastUpdated, Is.EqualTo(time));
			}
		}

		[Test]
		public void UpdatesManyReturnsNotFound()
		{
			var refunds = new[] {
				new Refund{Id = 1},
				new Refund{Id = 2},
			};
			Assert.That(() => _context.SaveChanges(() => _uut.UpdateMany(refunds)), Throws.TypeOf<NotFoundException>());
		}
	}
}
