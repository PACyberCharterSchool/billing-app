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

		[Test]
		[TestCase("DateOfBirthChange", null, "2000/12/12")]
		[TestCase("DistrictEnrollment", null, "234567890|Other SD")]
		[TestCase("DistrictWithdrawal", "123456789|Some SD", null)]
		[TestCase("NameChange", null, "George|C|Testy")]
		[TestCase("NameChange", null, "Bob|D|Testy")]
		[TestCase("NameChange", null, "Bob|C|Ytset")]
		[TestCase("GradeChange", null, "13")]
		[TestCase("AddressChange", null, "There Street|Apt 1|Some City|PA|12345")]
		[TestCase("AddressChange", null, "Here Street|Apt 2|Some City|PA|12345")]
		[TestCase("AddressChange", null, "Here Street|Apt 1|Other City|PA|12345")]
		[TestCase("AddressChange", null, "Here Street|Apt 1|Some City|MI|12345")]
		[TestCase("AddressChange", null, "Here Street|Apt 1|Some City|PA|54321")]
		[TestCase("SpecialEducationEnrollment", null, "True")]
		[TestCase("SpecialEducationWithdrawal", null, "False")]
		[TestCase("CurrentIepChange", null, "2000/12/12")]
		[TestCase("FormerIepChange", null, "2000/12/12")]
		[TestCase("NorepChange", null, "2000/12/12")]
		[TestCase("PASecuredChange", null, "987654321")]
		[TestCase("PASecuredChange", null, null)]
		[TestCase("PASecuredChange", null, "")]
		[TestCase("PASecuredChange", null, "  ")]
		public void TransformUpdatesStudent(string activity, string previous, string next)
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

			var records = new[] {
				new StudentActivityRecord
				{
					PACyberId = paCyberId,
					Activity = StudentActivity.FromString(activity),
					PreviousData = previous,
					NextData = next,
					Timestamp = time,
				},
			};

			// TODO(Erik): verify property updates
			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));

			_students.Verify(ss => ss.CreateOrUpdate(It.Is<Student>(s => s.PACyberId == paCyberId)), Times.Once);
		}

		[Test]
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
				Returns(new SchoolDistrict { Aun = districtAun });

			var records = new[] {
				new StudentActivityRecord{
					PACyberId = paCyberId,
					Activity = StudentActivity.DistrictEnrollment,
					NextData = districtAun.ToString() + "|" + districtName,
				},
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].SchoolDistrict.Aun, Is.EqualTo(districtAun));
		}

		[Test]
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
				Returns(new SchoolDistrict { Aun = districtAun });

			var records = new[] {
				new StudentActivityRecord{
					PACyberId = paCyberId,
					Activity = StudentActivity.DistrictWithdrawal,
					PreviousData = districtAun.ToString() + "|" + districtName,
				},
			};

			var result = _uut.Transform(records).Select(s => (Student)s).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result[0].SchoolDistrict.Aun, Is.EqualTo(districtAun));
		}
	}
}
