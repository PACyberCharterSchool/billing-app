using Microsoft.Data.Sqlite;
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
	public class EnrollmentRepositoryTests
	{
		private static SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");

		private PacBillContext _context;
		private ILogger<EnrollmentRepository> _logger;

		private EnrollmentRepository _uut;

		private static PacBillContext NewContext()
		{
			// use SQLite to support views
			var context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseSqlite(_conn).Options);
			context.Database.Migrate();

			return context;
		}

		[SetUp]
		public void SetUp()
		{
			_conn.Open();
			_context = NewContext();

			_logger = new TestLogger<EnrollmentRepository>();

			_uut = new EnrollmentRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_conn.Close();
		}

		private static void AssertEnrollment(Enrollment actual, CommittedStudentStatusRecord record)
		{
			Assert.That(actual.PACyberId, Is.EqualTo(record.StudentId));
			Assert.That(actual.Aun, Is.EqualTo(record.SchoolDistrictId));
			Assert.That(actual.StartDate, Is.EqualTo(record.StudentEnrollmentDate));
			Assert.That(actual.EndDate, Is.EqualTo(record.StudentWithdrawalDate));
			Assert.That(actual.IsSpecialEducation, Is.EqualTo(record.StudentIsSpecialEducation));
		}

		[Test]
		public void GetManyGetsByDateRange()
		{
			var time = DateTime.Now.Date;
			var records = new[] {
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddMonths(-1),
					StudentWithdrawalDate = time,
					StudentIsSpecialEducation = false,
				},
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddYears(-1),
					StudentWithdrawalDate = time.AddMonths(-2),
					StudentIsSpecialEducation = false,
				}
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(records));

			var results = _uut.GetMany(time.AddMonths(-1), time).ToList();
			Assert.That(results, Has.Count.EqualTo(1));
			AssertEnrollment(results[0], records[0]);
		}

		[Test]
		public void GetManyGetsByDateRangeWhenEndDateIsNull()
		{
			var time = DateTime.Now.Date;
			var records = new[] {
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(records));

			var results = _uut.GetMany(time.AddMonths(-1), time).ToList();
			Assert.That(results, Has.Count.EqualTo(1));
			AssertEnrollment(results[0], records[0]);
		}

		[Test]
		public void GetManyFiltersByPACyberId()
		{
			var paCyberId = "123456";
			var time = DateTime.Now.Date;
			var records = new[] {
				new CommittedStudentStatusRecord
				{
					StudentId = paCyberId,
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
				new CommittedStudentStatusRecord
				{
					StudentId = "234567",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(records));

			var results = _uut.GetMany(start: time.AddMonths(-1), end: time, paCyberId: paCyberId).ToList();
			Assert.That(results, Has.Count.EqualTo(1));
			AssertEnrollment(results[0], records[0]);
		}

		[Test]
		public void GetManyFiltersByAun()
		{
			var aun = 123456789;
			var time = DateTime.Now.Date;
			var records = new[] {
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = aun,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 234567890,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(records));

			var results = _uut.GetMany(start: time.AddMonths(-1), end: time, aun: aun).ToList();
			Assert.That(results, Has.Count.EqualTo(1));
			AssertEnrollment(results[0], records[0]);
		}

		[Test]
		public void GetManyDoesNotGetWhenStartEndSame()
		{
			var time = DateTime.Now.Date;
			var records = new[]
			{
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddMonths(-2),
					StudentWithdrawalDate = null,
					StudentIsSpecialEducation = false,
				},
				new CommittedStudentStatusRecord
				{
					StudentId = "123456",
					SchoolDistrictId = 123456789,
					StudentEnrollmentDate = time.AddDays(-7),
					StudentWithdrawalDate = time.AddDays(-7),
					StudentIsSpecialEducation = false,
				},
			};
			using (var ctx = NewContext())
				ctx.SaveChanges(() => ctx.AddRange(records));

			var results = _uut.GetMany(time.AddMonths(-1), time.AddMonths(1)).ToList();
			Assert.That(results, Has.Count.EqualTo(1));
			AssertEnrollment(results[0], records[0]);
		}
	}
}
