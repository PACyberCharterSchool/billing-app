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
	public class StudentActivityRecordRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<StudentActivityRecordRepository> _logger;

		private StudentActivityRecordRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("student-activity-records").Options);
			_logger = new TestLogger<StudentActivityRecordRepository>();

			_uut = new StudentActivityRecordRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void CreateWithNewCreates()
		{
			var record = new StudentActivityRecord
			{
				PACyberId = 123456789,
				Activity = StudentActivity.NEW_STUDENT,
				PreviousData = null,
				NextData = null,
				Timestamp = DateTime.Now.Date,
				BatchHash = "hash",
			};

			var result = _context.SaveChanges(() => _uut.Create(record));
			Assert.That(result, Is.EqualTo(record));

			var actual = _context.StudentActivityRecords.First(r => r.Id == result.Id);
			Assert.That(actual, Is.EqualTo(record));
		}

		[Test]
		public void CreateWithExistingFails()
		{
			var id = 1;
			var record = new StudentActivityRecord { Id = id };
			_context.Add(record);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.Create(record)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void CreateManyWithNewCreates()
		{
			var time = DateTime.Now;
			var records = new[] {
				new StudentActivityRecord(),
				new StudentActivityRecord(),
				new StudentActivityRecord(),
			};

			var result = _context.SaveChanges(() => _uut.CreateMany(records));
			Assert.That(result, Is.EqualTo(records));

			var actual = _context.StudentActivityRecords.ToList();
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public void CreateManyWithExistingFails()
		{
			var records = new[]{
				new StudentActivityRecord{Id = 1},
				new StudentActivityRecord{Id = 2},
				new StudentActivityRecord{Id = 3},
			};
			_context.AddRange(records);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(records)), Throws.TypeOf<ArgumentException>());
		}
	}
}
