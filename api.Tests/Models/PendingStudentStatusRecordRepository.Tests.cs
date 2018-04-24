using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using api.Models;
using api.Tests.Util;

namespace api.Tests.Models
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
				new StudentStatusRecord{Id = 1},
				new StudentStatusRecord{Id = 2},
				new StudentStatusRecord{Id = 3},
			};
			_context.PendingStudentStatusRecords.AddRange(records);
			_context.SaveChanges();

			var actual = _uut.GetMany();
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual, Is.EqualTo(records));
		}
	}
}
