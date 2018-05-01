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
	public class CommittedStudentStatusRecordRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<CommittedStudentStatusRecordRepository> _logger;

		private CommittedStudentStatusRecordRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("committed-student-status-records").Options);
			_logger = new TestLogger<CommittedStudentStatusRecordRepository>();

			_uut = new CommittedStudentStatusRecordRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void CreateWithNewCreates()
		{
			var time = DateTime.Now;
			var record = new CommittedStudentStatusRecord
			{
				SchoolDistrictId = 123456789,
				SchoolDistrictName = "Some Name",
				StudentId = 234567890,
				StudentFirstName = "Bob",
				StudentMiddleInitial = "C",
				StudentLastName = "Testy",
				StudentGradeLevel = "12",
				StudentDateOfBirth = DateTime.Now.AddYears(-18),
				StudentStreet1 = "Some Street",
				StudentStreet2 = "Some Apt",
				StudentCity = "Some City",
				StudentState = "Some State",
				StudentZipCode = "12345",
				ActivitySchoolYear = "2017-2018",
				StudentEnrollmentDate = DateTime.Now.AddYears(-2),
				StudentWithdrawalDate = DateTime.Now.AddYears(-1),
				StudentIsSpecialEducation = false,
				StudentCurrentIep = DateTime.Now.AddMonths(-1),
				StudentFormerIep = DateTime.Now.AddMonths(-2),
				StudentNorep = DateTime.Now.AddDays(-1),
				StudentPaSecuredId = 3456789012,
				BatchTime = DateTime.Now,
				BatchFilename = "file.csv",
				BatchHash = "hash",
			};

			var result = _context.SaveChanges(() => _uut.Create(time, record));
			Assert.That(result, Is.EqualTo(record));

			var actual = _context.CommittedStudentStatusRecords.First(r => r.Id == result.Id);
			Assert.That(actual, Is.EqualTo(record));
			Assert.That(actual.CommitTime, Is.EqualTo(time));
		}

		[Test]
		public void CreateWithExistingFails()
		{
			var id = 1;
			var record = new CommittedStudentStatusRecord { Id = id };
			_context.CommittedStudentStatusRecords.Add(record);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.Create(record)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void CreateManyWithNewCreates()
		{
			var time = DateTime.Now;
			var records = new[] {
				new CommittedStudentStatusRecord(),
				new CommittedStudentStatusRecord(),
				new CommittedStudentStatusRecord(),
			};

			var result = _context.SaveChanges(() => _uut.CreateMany(time, records));
			Assert.That(result, Is.EqualTo(records));

			var actual = _context.CommittedStudentStatusRecords.ToList();
			Assert.That(actual, Is.EqualTo(records));
			foreach (var a in actual)
				Assert.That(a.CommitTime, Is.EqualTo(time));
		}

		[Test]
		public void CreateManyWithExistingFails()
		{
			var records = new[]{
				new CommittedStudentStatusRecord{Id = 1},
				new CommittedStudentStatusRecord{Id = 2},
				new CommittedStudentStatusRecord{Id = 3},
			};
			_context.CommittedStudentStatusRecords.AddRange(records);
			_context.SaveChanges();

			Assert.That(() => _context.SaveChanges(() => _uut.CreateMany(records)), Throws.TypeOf<ArgumentException>());
		}
	}
}
