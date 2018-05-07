using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class PendingStudentStatusRecordRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<PendingStudentStatusRecordRepository> _logger;

		private PendingStudentStatusRecordRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("pending_student_status_records").Options);
			_logger = new TestLogger<PendingStudentStatusRecordRepository>();

			_uut = new PendingStudentStatusRecordRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void GetManyReturnsAll()
		{
			var records = new[]{
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_context.PendingStudentStatusRecords.AddRange(records);
			_context.SaveChanges();

			var actual = _uut.GetMany().ToList();
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public void GetManyWithSkipSkips()
		{
			var records = new[]{
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_context.PendingStudentStatusRecords.AddRange(records);
			_context.SaveChanges();

			var actual = _uut.GetMany(skip: 1).ToList();
			Assert.That(actual, Has.Count.EqualTo(2));
			Assert.That(actual[0], Is.EqualTo(records[1]));
			Assert.That(actual[1], Is.EqualTo(records[2]));
		}

		[Test]
		public void GetManyWithTakeTakes()
		{
			var records = new[]{
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_context.PendingStudentStatusRecords.AddRange(records);
			_context.SaveChanges();

			var actual = _uut.GetMany(take: 1).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0], Is.EqualTo(records[0]));
		}
	}
}
