using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System;

using Newtonsoft.Json;
using NUnit.Framework;

using models.Reporters;

namespace models.Tests.Reporters
{
	[TestFixture]
	public class InvoiceReporterTests
	{
		private PacBillContext _context;
		private InvoiceReporter _uut;

		private readonly SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");
		public PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().UseSqlite(_conn).Options);
			ctx.Database.Migrate();
			return ctx;
		}

		[SetUp]
		public void SetUp()
		{
			_conn.Open();
			_context = NewContext();

			_uut = new InvoiceReporter(_context);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_conn.Close();
		}

		[Test]
		public void GenerateReportGeneratesReport()
		{
			var time = new DateTime(2018, 2, 1);
			var aun = 123456789;
			var schoolYear = "2017-2018";

			var schoolDistrict = new SchoolDistrict
			{
				Aun = aun,
				Name = "Some SD",
				Rate = 1.23m,
			};
			using (var ctx = NewContext())
			{
				ctx.Add(schoolDistrict);

				ctx.AddRange(new[] {
					new CommittedStudentStatusRecord {
						StudentId = "123456",
						StudentPaSecuredId = 1234567890,
						SchoolDistrictId = aun,
						StudentFirstName = "Alice",
						StudentMiddleInitial = "B",
						StudentLastName = "Charlie",
						StudentStreet1 = "Somewhere",
						StudentCity = "Over The Rainbow",
						StudentState = "PA",
						StudentZipCode = "15000",
						StudentGradeLevel = "K",
						StudentDateOfBirth = time.AddYears(-5),
						StudentEnrollmentDate = time.AddYears(-1),
						StudentIsSpecialEducation = false,
					},
					new CommittedStudentStatusRecord {
						StudentId = "234567",
						StudentPaSecuredId = 2345678901,
						SchoolDistrictId = aun,
						StudentFirstName = "Bob",
						StudentMiddleInitial = "C",
						StudentLastName = "Doug",
						StudentStreet1 = "Not",
						StudentStreet2 = "There",
						StudentCity = "Anywhere",
						StudentState = "PA",
						StudentZipCode = "15000",
						StudentGradeLevel = "12",
						StudentDateOfBirth = time.AddYears(-18),
						StudentEnrollmentDate = time.AddYears(-1),
						StudentWithdrawalDate = time.AddMonths(-6),
						StudentCurrentIep = time.AddYears(-1),
						StudentIsSpecialEducation = true,
					},
					new CommittedStudentStatusRecord {
						StudentId = "345678",
						SchoolDistrictId = aun,
						StudentEnrollmentDate = time.AddMonths(-1),
						StudentWithdrawalDate = time.AddMonths(-1),
						StudentIsSpecialEducation = false,
					},
				});

				ctx.AddRange(new[] {
					new Payment {
						Type = PaymentType.Check,
						ExternalId = "09876",
						Amount = 100m,
						Date = new DateTime(2017, 8, 10),
						SchoolYear = schoolYear,
						SchoolDistrict = schoolDistrict,
					},
					new Payment {
						Type = PaymentType.UniPay,
						Amount = 200m,
						Date = new DateTime(2018, 1, 10),
						SchoolYear = schoolYear,
						SchoolDistrict = schoolDistrict,
					}
				});

				ctx.Add(new Refund
				{
					Amount = 150m,
					SchoolYear = schoolYear,
					SchoolDistrict = schoolDistrict,
					Date = new DateTime(2017, 10, 23),
				});

				ctx.SaveChanges();
			}

			var actual = _uut.GenerateReport(new InvoiceReporter.Config
			{
				InvoiceNumber = "1234567890",
				SchoolYear = schoolYear,
				Prepared = time,
				ToSchoolDistrict = time.AddDays(1),
				ToPDE = time.AddDays(10),
				SchoolDistrictAun = aun,
			});

			Console.WriteLine($"actual: {JsonConvert.SerializeObject(actual, Formatting.Indented)}");
			Assert.Fail("output");
		}
	}
}
