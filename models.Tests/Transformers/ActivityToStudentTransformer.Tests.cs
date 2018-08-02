using System;
using System.Linq;

using Moq;
using NUnit.Framework;

using models;
using models.Transformers;

namespace models.Tests.Transformers
{
	[TestFixture]
	public class ActivityToStudentTransformerTests
	{
		private Mock<IStudentRepository> _students;
		private Mock<ISchoolDistrictRepository> _districts;

		private ActivityToStudentTransformer _uut;

		[SetUp]
		public void SetUp()
		{
			_students = new Mock<IStudentRepository>();
			_districts = new Mock<ISchoolDistrictRepository>();

			_uut = new ActivityToStudentTransformer(_students.Object, _districts.Object);
		}

		[Test]
		[Ignore("No time.")]
		public void TransformCreatesNewStudent()
		{
			var paCyberId = "3";
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns((Student)null);

			var records = new[] {
				new StudentActivityRecord{PACyberId = paCyberId, Activity = StudentActivity.NewStudent}
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].PACyberId, Is.EqualTo(paCyberId));

			_students.Verify(ss => ss.CreateOrUpdate(It.Is<Student>(s => s.PACyberId == paCyberId)), Times.Once);
		}

		private void TransformUpdates(
			StudentActivity activity,
			string previous,
			string next,
			Action<Student, StudentActivityRecord, Student> verify)
		{
			var paCyberId = "3";
			var time = DateTime.Now.Date;
			var student = new Student
			{
				Id = 1,
				PACyberId = paCyberId,
				PASecuredId = 123456789,
				FirstName = "Bob",
				MiddleInitial = "C",
				LastName = "Testy",
				Grade = "12",
				DateOfBirth = time.AddYears(-18),
				Street1 = "Here Street",
				Street2 = "Apt 1",
				City = "Some City",
				State = "PA",
				ZipCode = "12345",
				IsSpecialEducation = false,
				CurrentIep = time.AddMonths(-1),
				FormerIep = time.AddMonths(-2),
				NorepDate = time.AddMonths(-3),
				StartDate = time.AddMonths(-4),
				EndDate = null,
				Created = time.AddMonths(-5),
				LastUpdated = time.AddMonths(-5),
				SchoolDistrict = new SchoolDistrict
				{
					Aun = 123456789,
					Name = "Some SD",
				},
			};
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);
			_districts.Setup(ds => ds.GetByAun(It.IsAny<int>())).Returns<int>(aun => new SchoolDistrict
			{
				Aun = aun,
				Name = "DB SD",
			});
			_districts.Setup(ds => ds.CreateOrUpdate(It.IsAny<SchoolDistrict>())).Returns<SchoolDistrict>(x => x);

			var records = new[] {
				new StudentActivityRecord
				{
					PACyberId = paCyberId,
					Activity = activity,
					PreviousData = previous,
					NextData = next,
					Timestamp = time,
				},
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			verify(student, records[0], result[0]);

			_students.Verify(ss => ss.CreateOrUpdate(It.Is<Student>(s => s.PACyberId == paCyberId)), Times.Once);
		}

		[Test]
		[TestCase(null, "2000/12/12")]
		[Ignore("No time.")]
		public void TransformUpdatesDateOfBirth(string previous, string next) =>
			TransformUpdates(StudentActivity.DateOfBirthChange, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.DateOfBirth, Is.EqualTo(DateTime.Parse(next)));
			});

		[Test]
		[TestCase(null, "234567890|Other SD")]
		[Ignore("No time.")]
		public void TransformUpdatesDistrictEnrollment(string previous, string next) =>
			TransformUpdates(StudentActivity.DistrictEnrollment, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.StartDate, Is.EqualTo(record.Timestamp));
				Assert.That(actual.EndDate, Is.Null);
				Assert.That(actual.SchoolDistrict, Is.Not.Null);

				var parts = next.Split("|");
				var aun = int.Parse(parts[0]);
				var name = parts[1];
				Assert.That(actual.SchoolDistrict.Aun, Is.EqualTo(aun));
				Assert.That(actual.SchoolDistrict.Name, Is.EqualTo(name));
			});

		[Test]
		[TestCase("123456789|Some SD", null)]
		[Ignore("No time.")]
		public void TransformUpdatesDistrictWithdrawal(string previous, string next) =>
			TransformUpdates(StudentActivity.DistrictWithdrawal, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.EndDate, Is.EqualTo(record.Timestamp));
				Assert.That(actual.SchoolDistrict, Is.Not.Null);

				var parts = previous.Split("|");
				var aun = int.Parse(parts[0]);
				var name = parts[1];
				Assert.That(actual.SchoolDistrict.Aun, Is.EqualTo(aun));
				Assert.That(actual.SchoolDistrict.Name, Is.EqualTo(name));
			});

		[Test]
		[TestCase(null, "George|C|Testy")]
		[TestCase(null, "Bob|D|Testy")]
		[TestCase(null, "Bob|C|Ytset")]
		[Ignore("No time.")]
		public void TransformUpdatesName(string previous, string next) =>
			TransformUpdates(StudentActivity.NameChange, previous, next, (student, record, actual) =>
			{
				var parts = next.Split("|");
				Assert.That(actual.FirstName, Is.EqualTo(parts[0]));
				Assert.That(actual.MiddleInitial, Is.EqualTo(parts[1]));
				Assert.That(actual.LastName, Is.EqualTo(parts[2]));
			});

		[Test]
		[TestCase(null, "13")]
		[Ignore("No time.")]
		public void TransformUpdatesGrade(string previous, string next) =>
			TransformUpdates(StudentActivity.GradeChange, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.Grade, Is.EqualTo(next));
			});

		[Test]
		[TestCase(null, "There Street|Apt 1|Some City|PA|12345")]
		[TestCase(null, "Here Street|Apt 2|Some City|PA|12345")]
		[TestCase(null, "Here Street|Apt 1|Other City|PA|12345")]
		[TestCase(null, "Here Street|Apt 1|Some City|MI|12345")]
		[TestCase(null, "Here Street|Apt 1|Some City|PA|54321")]
		[Ignore("No time.")]
		public void TransformUpdatesAddress(string previous, string next) =>
			TransformUpdates(StudentActivity.AddressChange, previous, next, (student, record, actual) =>
			{
				var parts = next.Split("|");
				Assert.That(actual.Street1, Is.EqualTo(parts[0]));
				Assert.That(actual.Street2, Is.EqualTo(parts[1]));
				Assert.That(actual.City, Is.EqualTo(parts[2]));
				Assert.That(actual.State, Is.EqualTo(parts[3]));
				Assert.That(actual.ZipCode, Is.EqualTo(parts[4]));
			});

		[Test]
		[TestCase(null, null)]
		[Ignore("No time.")]
		public void TransformUpdatesSpecialEducationEnrollment(string previous, string next) =>
			TransformUpdates(StudentActivity.SpecialEducationEnrollment, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.IsSpecialEducation, Is.True);
			});

		[Test]
		[TestCase(null, null)]
		[Ignore("No time.")]
		public void TransformUpdatesSpecialEducationWithdrawal(string previous, string next) =>
			TransformUpdates(StudentActivity.SpecialEducationWithdrawal, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.IsSpecialEducation, Is.False);
			});

		[Test]
		[TestCase(null, "2000/12/12")]
		[Ignore("No time.")]
		public void TransformUpdatesCurrentIep(string previous, string next) =>
			TransformUpdates(StudentActivity.CurrentIepChange, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.CurrentIep, Is.EqualTo(DateTime.Parse(next)));
			});

		[Test]
		[TestCase(null, "2000/12/12")]
		[Ignore("No time.")]
		public void TransformUpdatesFormerIep(string previous, string next) =>
			TransformUpdates(StudentActivity.FormerIepChange, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.FormerIep, Is.EqualTo(DateTime.Parse(next)));
			});

		[Test]
		[TestCase(null, "2000/12/12")]
		[Ignore("No time.")]
		public void TransformUpdatesNorep(string previous, string next) =>
			TransformUpdates(StudentActivity.NorepChange, previous, next, (student, record, actual) =>
			{
				Assert.That(actual.NorepDate, Is.EqualTo(DateTime.Parse(next)));
			});

		[Test]
		[TestCase(null, "987654321")]
		[TestCase(null, null)]
		[TestCase(null, "")]
		[TestCase(null, "  ")]
		[Ignore("No time.")]
		public void TransformUpdatesPASecuredId(string previous, string next) =>
			TransformUpdates(StudentActivity.PASecuredChange, previous, next, (student, record, actual) =>
			{
				var id = string.IsNullOrWhiteSpace(next) ? (ulong?)null : ulong.Parse(next);
				Assert.That(actual.PASecuredId, Is.EqualTo(id));
			});

		[Test]
		[Ignore("No time.")]
		public void TransformCreatesNewSchoolDistrictDistrictEnrollment()
		{
			var paCyberId = "3";
			var student = new Student { PACyberId = paCyberId };
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var districtAun = 123456789;
			var districtName = "Some SD";
			_districts.Setup(ds => ds.GetByAun(districtAun)).Returns((SchoolDistrict)null);
			_districts.Setup(ds => ds.CreateOrUpdate(
				It.Is<SchoolDistrict>(d => d.Aun == districtAun && d.Name == districtName))).
				Returns<SchoolDistrict>(d => d);

			var records = new[] {
				new StudentActivityRecord{
					PACyberId = paCyberId,
					Activity = StudentActivity.DistrictEnrollment,
					NextData = districtAun.ToString() + "|" + districtName,
				}
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].SchoolDistrict.Aun, Is.EqualTo(districtAun));

			_districts.Verify(ds => ds.CreateOrUpdate(
				It.Is<SchoolDistrict>(d => d.Aun == districtAun && d.Name == districtName)), Times.Once);
		}

		[Test]
		[Ignore("No time.")]
		public void TransformCreatesNewSchoolDistrictDistrictWithdrawal()
		{
			var paCyberId = "3";
			var student = new Student { PACyberId = paCyberId };
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var districtAun = 123456789;
			var districtName = "Some SD";
			_districts.Setup(ds => ds.GetByAun(districtAun)).Returns((SchoolDistrict)null);
			_districts.Setup(ds => ds.CreateOrUpdate(
				It.Is<SchoolDistrict>(d => d.Aun == districtAun && d.Name == districtName))).
				Returns<SchoolDistrict>(d => d);

			var records = new[] {
				new StudentActivityRecord{
					PACyberId = paCyberId,
					Activity = StudentActivity.DistrictWithdrawal,
					PreviousData = districtAun.ToString() + "|" + districtName,
				},
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].SchoolDistrict, Is.Not.Null);
			Assert.That(result[0].SchoolDistrict.Aun, Is.EqualTo(districtAun));
			Assert.That(result[0].SchoolDistrict.Name, Is.EqualTo(districtName));

			_districts.Verify(ds => ds.CreateOrUpdate(
				It.Is<SchoolDistrict>(d => d.Aun == districtAun && d.Name == districtName)), Times.Once);
		}

		// See: https://pacyber.atlassian.net/browse/BA-176
		[Test]
		[Ignore("No time.")]
		public void TransformDoesntLoseStudentData()
		{
			var time = DateTime.Now;
			var student = new Student
			{
				Id = 1,
				PACyberId = "123456",
				PASecuredId = 123456789,
				FirstName = "Bob",
				MiddleInitial = "C",
				LastName = "Testy",
				Grade = "3",
				DateOfBirth = time.AddYears(-18),
				Street1 = "Here Street",
				Street2 = "Apt 1",
				City = "Some City",
				State = "PA",
				ZipCode = "12345",
				IsSpecialEducation = false,
				CurrentIep = time.AddMonths(-1),
				FormerIep = time.AddMonths(-2),
				NorepDate = time.AddMonths(-3),
				StartDate = time.AddMonths(-4),
				EndDate = null,
				Created = time.AddMonths(-5),
				LastUpdated = time.AddMonths(-5),
				SchoolDistrict = new SchoolDistrict
				{
					Aun = 123456789,
					Name = "Some SD",
				},
			};
			_students.Setup(ss => ss.GetByPACyberId(student.PACyberId)).Returns(student);

			var records = new[] {
				new StudentActivityRecord {
					PACyberId = student.PACyberId,
					Activity = StudentActivity.GradeChange,
					PreviousData = "3",
					NextData = "4",
					Timestamp = new DateTime(2014, 9, 2),
					BatchHash = "b715723702",
					Sequence = 1,
				},
			};

			var actuals = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(actuals, Has.Count.EqualTo(1));

			var actual = actuals[0];
			foreach (var p in typeof(Student).GetProperties())
			{
				if (p.Name == nameof(Student.Grade))
				{
					Assert.That(actual.Grade, Is.EqualTo("4"));
					continue;
				}

				Assert.That(p.GetValue(actual), Is.EqualTo(p.GetValue(student)));
			}
		}
	}
}
