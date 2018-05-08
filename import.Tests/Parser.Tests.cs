using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using models;

namespace import.Tests
{
	[TestFixture]
	public class ImporterTests
	{
		private const string HEADER = "StudentIndex,EnrollmentRecordIndex,StudentAddressIndex,DistrictListIndex,school_name,schooldistrict,student_id,last_name,first_name,middle_initial,grade_level,date_of_birth,home_street1,home_Street2,home_city,home_state,home_zip,School_Year,first_date,last_date,Education_Code,Current_IEP_Date,Former_IEP_Date,NORP_Date,pa_secured_id";

		private Parser _uut;

		[SetUp]
		public void SetUp()
		{
			_uut = new Parser();
		}

		[Test]
		public void ParseReturnsRecords()
		{
			var time = DateTime.Now;
			var record = new PendingStudentStatusRecord
			{
				SchoolDistrictId = 123456789,
				SchoolDistrictName = "Some SD",
				StudentId = "234567890",
				StudentFirstName = "Bob",
				StudentMiddleInitial = "C",
				StudentLastName = "Testy",
				StudentGradeLevel = "12",
				StudentDateOfBirth = time.AddYears(-18).Date,
				StudentStreet1 = "Some Street",
				StudentStreet2 = "Some Apt",
				StudentCity = "Some City",
				StudentState = "Some State",
				StudentZipCode = "12345",
				ActivitySchoolYear = "2017-2018",
				StudentEnrollmentDate = time.AddMonths(-2).Date,
				StudentWithdrawalDate = time.AddMonths(-1).Date,
				StudentIsSpecialEducation = false,
				StudentCurrentIep = time.AddDays(-1).Date,
				StudentFormerIep = time.AddYears(-2).Date,
				StudentNorep = time.AddHours(-1).Date,
				StudentPaSecuredId = 345678901,
			};

			var sb = new StringBuilder();
			sb.Append($"{HEADER}\n");
			sb.Append($"{record.StudentId},"); // StudentIndex
			sb.Append("2,"); // EnrollmentRecordIndex
			sb.Append("3,"); // StudentAddressIndex
			sb.Append("4,"); // DistrictListIndex
			sb.Append($"{record.SchoolDistrictName},"); // school name
			sb.Append($"{record.SchoolDistrictId},"); // schooldistrict
			sb.Append("1,"); // student_id
			sb.Append($"{record.StudentLastName},"); // last_name
			sb.Append($"{record.StudentFirstName},"); // first_name
			sb.Append($"{record.StudentMiddleInitial},"); // middle_initial
			sb.Append($"{record.StudentGradeLevel},"); // grade_level
			sb.Append($"{record.StudentDateOfBirth},"); // date_of_birth
			sb.Append($"{record.StudentStreet1},"); // home_street1
			sb.Append($"{record.StudentStreet2},"); // home_Street2
			sb.Append($"{record.StudentCity},"); // home_city
			sb.Append($"{record.StudentState},"); // home_state
			sb.Append($"{record.StudentZipCode},"); // home_zip
			sb.Append($"{record.ActivitySchoolYear},"); // School_Year
			sb.Append($"{record.StudentEnrollmentDate},"); // first_date
			sb.Append($"{record.StudentWithdrawalDate},"); // last_date
			sb.Append($"{(record.StudentIsSpecialEducation == true ? "Y" : "N")},"); // Education_Code
			sb.Append($"{record.StudentCurrentIep},"); // Current_IEP_Date
			sb.Append($"{record.StudentFormerIep},"); // Former_IEP_Date
			sb.Append($"{record.StudentNorep},"); // NORP_Date
			sb.Append($"{record.StudentPaSecuredId}\n"); // pa_secured_id
			var csv = sb.ToString();
			Console.WriteLine($"csv:\n{csv}");

			var filename = "test.csv";
			var actual = _uut.Parse(new StringReader(csv), filename).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].Id, Is.EqualTo(2)); // sets raw row
			Assert.That(actual[0].SchoolDistrictId, Is.EqualTo(record.SchoolDistrictId));
			Assert.That(actual[0].SchoolDistrictName, Is.EqualTo(record.SchoolDistrictName));
			Assert.That(actual[0].StudentId, Is.EqualTo(record.StudentId));
			Assert.That(actual[0].StudentFirstName, Is.EqualTo(record.StudentFirstName));
			Assert.That(actual[0].StudentMiddleInitial, Is.EqualTo(record.StudentMiddleInitial));
			Assert.That(actual[0].StudentLastName, Is.EqualTo(record.StudentLastName));
			Assert.That(actual[0].StudentGradeLevel, Is.EqualTo(record.StudentGradeLevel));
			Assert.That(actual[0].StudentDateOfBirth, Is.EqualTo(record.StudentDateOfBirth));
			Assert.That(actual[0].StudentStreet1, Is.EqualTo(record.StudentStreet1));
			Assert.That(actual[0].StudentStreet2, Is.EqualTo(record.StudentStreet2));
			Assert.That(actual[0].StudentCity, Is.EqualTo(record.StudentCity));
			Assert.That(actual[0].StudentState, Is.EqualTo(record.StudentState));
			Assert.That(actual[0].StudentZipCode, Is.EqualTo(record.StudentZipCode));
			Assert.That(actual[0].ActivitySchoolYear, Is.EqualTo(record.ActivitySchoolYear));
			Assert.That(actual[0].StudentEnrollmentDate, Is.EqualTo(record.StudentEnrollmentDate));
			Assert.That(actual[0].StudentWithdrawalDate, Is.EqualTo(record.StudentWithdrawalDate));
			Assert.That(actual[0].StudentIsSpecialEducation, Is.EqualTo(record.StudentIsSpecialEducation));
			Assert.That(actual[0].StudentCurrentIep, Is.EqualTo(record.StudentCurrentIep));
			Assert.That(actual[0].StudentFormerIep, Is.EqualTo(record.StudentFormerIep));
			Assert.That(actual[0].StudentNorep, Is.EqualTo(record.StudentNorep));
			Assert.That(actual[0].StudentPaSecuredId, Is.EqualTo(record.StudentPaSecuredId));
			Assert.That(actual[0].BatchFilename, Is.EqualTo(filename));
			Assert.That(actual[0].BatchTime.ToString(), Is.EqualTo(time.ToString()));
			Assert.That(actual[0].BatchHash, Is.Not.Empty);
		}
	}
}
