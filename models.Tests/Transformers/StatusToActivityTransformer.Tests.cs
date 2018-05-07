using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;
using NUnit.Framework;

using models;
using models.Transformers;

namespace models.Tests.Transformers
{
	[TestFixture]
	public class StatusToActivityTransformerTests
	{
		private Mock<IStudentRepository> _students;
		private Mock<IStudentActivityRecordRepository> _activities;

		private StatusToActivityTransformer _uut;

		[SetUp]
		public void SetUp()
		{
			_students = new Mock<IStudentRepository>();
			_activities = new Mock<IStudentActivityRecordRepository>();

			_uut = new StatusToActivityTransformer(_students.Object, _activities.Object);
		}

		private static Student NewStudent(string paCyberId) => new Student
		{
			PACyberId = paCyberId,
			PASecuredId = null,
			FirstName = "First",
			MiddleInitial = "M",
			LastName = "Last",
			Grade = "0",
			DateOfBirth = DateTime.Now.AddYears(-18),
			Street1 = "Street1",
			Street2 = "Street2",
			City = "City",
			State = "State",
			ZipCode = "Zip",
			IsSpecialEducation = false,
			CurrentIep = null,
			FormerIep = null,
			NorepDate = null,
			StartDate = DateTime.Now.AddMonths(-6),
			EndDate = null,
			SchoolDistrict = new SchoolDistrict
			{
				Aun = 123456789,
				Name = "Some SD",
			},
		};

		private static StudentStatusRecord NewStatusRecord(DateTime enrollDate, string batchHash, Student student)
			=> new StudentStatusRecord
			{
				StudentId = student.PACyberId,
				StudentEnrollmentDate = enrollDate,
				BatchHash = batchHash,

				SchoolDistrictId = student.SchoolDistrict.Aun,
				SchoolDistrictName = student.SchoolDistrict.Name,
				StudentFirstName = student.FirstName,
				StudentMiddleInitial = student.MiddleInitial,
				StudentLastName = student.LastName,
				StudentGradeLevel = student.Grade,
				StudentDateOfBirth = student.DateOfBirth,
				StudentStreet1 = student.Street1,
				StudentStreet2 = student.Street2,
				StudentCity = student.City,
				StudentState = student.State,
				StudentZipCode = student.ZipCode,
				ActivitySchoolYear = "Year",
				StudentWithdrawalDate = student.EndDate,
				StudentIsSpecialEducation = student.IsSpecialEducation,
				StudentCurrentIep = student.CurrentIep,
				StudentFormerIep = student.FormerIep,
				StudentNorep = student.NorepDate,
				StudentPaSecuredId = student.PASecuredId,
				BatchTime = DateTime.Now,
				BatchFilename = "test.csv",
			};

		[Test]
		public void TransformCreatesNewStudentRecordsIfNotExists()
		{
			var paCyberId = "3";
			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] {
				NewStatusRecord(enrollDate, batchHash, NewStudent(paCyberId)),
		};

			_students.Setup(s => s.GetByPACyberId(paCyberId)).Returns((Student)null);

			var activities = new[] {
				StudentActivity.NewStudent,
				StudentActivity.DateOfBirthChange,
				StudentActivity.NameChange,
				StudentActivity.GradeChange,
				StudentActivity.AddressChange,
				StudentActivity.DistrictEnrollment,
			};

			var actual = _uut.Transform(statuses).Select(s => (StudentActivityRecord)s).ToList();
			Assert.That(actual, Has.Count.EqualTo(activities.Length));

			for (int i = 0; i < activities.Length; i++)
			{
				var record = actual[i];
				Console.WriteLine($"activity: {record.Activity}");
				var expected = activities[i];
				Assert.That(record.Activity, Is.EqualTo(expected));
				Assert.That(record.PACyberId, Is.EqualTo(paCyberId));
				Assert.That(record.Timestamp, Is.EqualTo(enrollDate));
				Assert.That(record.BatchHash, Is.EqualTo(batchHash));

				var sequence = i + 1;
				Assert.That(actual[i].Sequence, Is.EqualTo(sequence));
			}

			_activities.Verify(
				a => a.Create(It.Is<StudentActivityRecord>(r => r.PACyberId == paCyberId)),
						Times.Exactly(activities.Length));
		}

		[Test]
		public void TransformDoesNotCreateDuplicatesIfNotExists()
		{
			var paCyberId = "3";
			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] {
				NewStatusRecord(enrollDate, batchHash, NewStudent(paCyberId)),
				NewStatusRecord(enrollDate, batchHash, NewStudent(paCyberId)),
			};

			_students.Setup(s => s.GetByPACyberId(paCyberId)).Returns((Student)null);

			var count = 6;
			var actual = _uut.Transform(statuses).Select(s => (StudentActivityRecord)s).ToList();
			Assert.That(actual, Has.Count.EqualTo(count));
		}

		private static void Assign<T>(T target, string propName, object value)
		{
			var param = Expression.Parameter(typeof(T), "x");
			var prop = Expression.PropertyOrField(param, propName);
			var type = prop.Type;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = Nullable.GetUnderlyingType(type);

			var lambda = Expression.Lambda<Action<T>>(
				body: Expression.Assign(
					left: prop,
					right: Expression.Convert(Expression.Constant(Convert.ChangeType(value, type)), prop.Type)
				),
				parameters: param
			);
			Console.WriteLine($"lambda: {lambda}");
			lambda.Compile()(target);
		}

		[Test]
		[TestCase("DateOfBirthChange", "DateOfBirth", "2000/01/01", "StudentDateOfBirth", "2000/02/02")]
		[TestCase("GradeChange", "Grade", "11", "StudentGradeLevel", "12")]
		[TestCase("NorepChange", "NorepDate", "2018/01/01", "StudentNorep", "2018/02/02")]
		[TestCase("PASecuredChange", "PASecuredId", 123456789, "StudentPASecuredId", 234567890)]
		[TestCase("CurrentIepChange", "CurrentIep", "2018/01/01", "StudentCurrentIep", "2018/02/02")]
		[TestCase("FormerIepChange", "FormerIep", "2018/01/01", "StudentFormerIep", "2018/02/02")]
		public void TransformWithExistingStudentCreatesRecordIfChanged<T>(
			string activityString,
			string studentProperty, T oldData,
			string recordProperty, T newData)
		{
			var activity = StudentActivity.FromString(activityString);
			Console.WriteLine($"activity: {activity}");

			var paCyberId = "3";
			var student = NewStudent(paCyberId);
			Assign(student, studentProperty, oldData);
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, student) };
			Assign(statuses[0], recordProperty, newData);

			var actual = _uut.Transform(statuses).Select(s => (StudentActivityRecord)s).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[0].Activity, Is.EqualTo(activity));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldData.ToString()));
			Assert.That(actual[0].NextData, Is.EqualTo(newData.ToString()));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformReturnsNullForDefaultDateTimePreviousData()
		{
			var paCyberId = "3";
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns((Student)null);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, NewStudent(paCyberId)) };
			statuses[0].StudentDateOfBirth = DateTime.Parse("2000/01/01");

			var actual = _uut.Transform(statuses).Select(s => (StudentActivityRecord)s).ToList();
			Assert.That(actual, Has.Count.EqualTo(6));
			Assert.That(actual[1].Activity, Is.EqualTo(StudentActivity.DateOfBirthChange));
			Assert.That(actual[1].PreviousData, Is.Null);
		}

		[Test]
		[TestCase("NameChange", "FirstName", "Bob", "StudentFirstName", "Charlie")]
		[TestCase("NameChange", "MiddleInitial", "A", "StudentMiddleInitial", "B")]
		[TestCase("NameChange", "LastName", "Ytset", "StudentLastName", "Testy")]
		public void TransformWithExistingStudentCreatesNameChangeRecordIfChanged(
					string activityString,
					string studentProperty, string oldData,
					string recordProperty, string newData)
		{
			var activity = StudentActivity.FromString(activityString);

			var paCyberId = "3";
			var student = NewStudent(paCyberId);
			Assign(student, studentProperty, oldData);
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var oldName = String.Join("|", student.FirstName, student.MiddleInitial, student.LastName);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, student) };
			Assign(statuses[0], recordProperty, newData);

			var newName = String.Join("|",
				statuses[0].StudentFirstName,
				statuses[0].StudentMiddleInitial,
				statuses[0].StudentLastName);

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[0].Activity, Is.EqualTo(activity));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldName));
			Assert.That(actual[0].NextData, Is.EqualTo(newName));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		[TestCase("AddressChange", "Street1", "Here", "StudentStreet1", "There")]
		[TestCase("AddressChange", "Street2", "Here", "StudentStreet2", "There")]
		[TestCase("AddressChange", "City", "East Side", "StudentCity", "West Side")]
		[TestCase("AddressChange", "State", "PA", "StudentState", "Not PA")]
		[TestCase("AddressChange", "ZipCode", "12345", "StudentZipCode", "67890")]
		public void TransformWithExistingStudentCreatesAddressChangeRecordIfChanged(
					string activityString,
					string studentProperty, string oldData,
					string recordProperty, string newData)
		{
			var activity = StudentActivity.FromString(activityString);

			var paCyberId = "3";
			var student = NewStudent(paCyberId);
			Assign(student, studentProperty, oldData);
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var oldAddress = String.Join("|",
				student.Street1,
				student.Street2,
				student.City,
				student.State,
				student.ZipCode);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, student) };
			Assign(statuses[0], recordProperty, newData);

			var newAddress = String.Join("|",
				statuses[0].StudentStreet1,
				statuses[0].StudentStreet2,
				statuses[0].StudentCity,
				statuses[0].StudentState,
				statuses[0].StudentZipCode);

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[0].Activity, Is.EqualTo(activity));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldAddress));
			Assert.That(actual[0].NextData, Is.EqualTo(newAddress));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatesDistrictEnrollRecordIfChanged()
		{
			var studentId = "3";
			var previousAun = 4;
			var previousName = "Old SD";
			_students.Setup(ss => ss.GetByPACyberId(studentId)).Returns(new Student
			{
				PACyberId = studentId,
				SchoolDistrict = new SchoolDistrict
				{
					Aun = previousAun,
					Name = previousName,
				},
			});

			var oldDistrict = String.Join("|", previousAun, previousName);

			var nextAun = 5;
			var nextName = "New SD";
			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] {
				new StudentStatusRecord{
					StudentId = studentId,
					SchoolDistrictId = nextAun,
					SchoolDistrictName = nextName,
					StudentEnrollmentDate = enrollDate,
					BatchHash = batchHash,
				},
			};

			var newDistrict = String.Join("|", nextAun, nextName);

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.DistrictEnrollment));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldDistrict));
			Assert.That(actual[0].NextData, Is.EqualTo(newDistrict));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatesDistrictWithdrawRecordIfPresent()
		{
			var studentId = "3";
			var districtAun = 4;
			_students.Setup(ss => ss.GetByPACyberId(studentId)).Returns(new Student
			{
				PACyberId = studentId,
				SchoolDistrict = new SchoolDistrict { Aun = districtAun },
			});

			var enrollDate = DateTime.Now.Date.AddMonths(-6);
			var withdrawDate = DateTime.Now;
			var batchHash = "hash";
			var statuses = new[] {
				new StudentStatusRecord{
					StudentId = studentId,
					SchoolDistrictId = districtAun,
					StudentEnrollmentDate = enrollDate,
					StudentWithdrawalDate = withdrawDate,
					BatchHash = batchHash,
				},
			};

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.DistrictWithdrawal));
			Assert.That(actual[0].Timestamp, Is.EqualTo(withdrawDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(districtAun.ToString() + "|"));
			Assert.That(actual[0].NextData, Is.EqualTo(null));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatesSpecialWithdrawRecordIfPresent()
		{
			var paCyberId = "3";
			var student = NewStudent(paCyberId);
			student.IsSpecialEducation = true;
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, student) };
			statuses[0].StudentIsSpecialEducation = false;
			statuses[0].StudentWithdrawalDate = DateTime.Now.Date;

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(2)); // district withdrawal is first
			Assert.That(actual[1].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[1].Activity, Is.EqualTo(StudentActivity.SpecialEducationWithdrawal));
			Assert.That(actual[1].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[1].PreviousData, Is.EqualTo(null));
			Assert.That(actual[1].NextData, Is.EqualTo(null));
			Assert.That(actual[1].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatestSpecialEnrollmentRecordIfPresent()
		{
			var paCyberId = "3";
			var student = NewStudent(paCyberId);
			student.IsSpecialEducation = false;
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(enrollDate, batchHash, student) };
			statuses[0].StudentIsSpecialEducation = true;

			var actual = _uut.Transform(statuses).Select(s => (StudentActivityRecord)s).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.SpecialEducationEnrollment));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(null));
			Assert.That(actual[0].NextData, Is.EqualTo(null));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}
	}
}
