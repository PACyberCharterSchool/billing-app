using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class AuditRecordRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<AuditRecordRepository> _logger;

		private AuditRecordRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("audit-records").Options);
			_logger = new TestLogger<AuditRecordRepository>();

			_uut = new AuditRecordRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void CreateWithNewCreates()
		{
			var record = new AuditRecord
			{
				Username = "Bob",
				Activity = AuditRecordActivity.EDIT_STUDENT_RECORD,
			};

			var time = DateTime.Now;
			var result = _context.SaveChanges(() => _uut.Create(time, record));
			Assert.That(result, Is.EqualTo(record));

			var actual = _context.AuditRecords.First(r => r.Id == result.Id);
			Assert.That(actual, Is.EqualTo(record));
			Assert.That(actual.Timestamp, Is.EqualTo(time));
		}

		[Test]
		public void CreateWithExistingFails()
		{
			var id = 1;
			var record = new AuditRecord { Id = id };
			_context.Add(record);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.Create(record)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void CreateManyWithNewCreates()
		{
			var time = DateTime.Now;
			var records = new[] {
				new AuditRecord(),
				new AuditRecord(),
				new AuditRecord(),
			};

			var result = _context.SaveChanges(() => _uut.CreateMany(time, records));
			Assert.That(result, Is.EqualTo(records));

			var actual = _context.AuditRecords.ToList();
			Assert.That(actual, Is.EqualTo(records));
			foreach (var a in actual)
				Assert.That(a.Timestamp, Is.EqualTo(time));
		}

		[Test]
		public void CreateManyWithExistingFails()
		{
			var records = new[]{
				new AuditRecord{Id = 1},
				new AuditRecord{Id = 2},
				new AuditRecord{Id = 3},
			};
			_context.AddRange(records);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(records)), Throws.TypeOf<ArgumentException>());
		}
	}
}
