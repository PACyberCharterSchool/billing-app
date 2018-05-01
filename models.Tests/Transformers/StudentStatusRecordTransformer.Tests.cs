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
	public class StudentStatusRecordTransformerTests
	{
		private Mock<IStudentRepository> _students;
		private Mock<IStudentActivityRecordRepository> _activities;

		private StudentStatusRecordTransformer _uut;

		[SetUp]
		public void SetUp()
		{
			_students = new Mock<IStudentRepository>();
			_activities = new Mock<IStudentActivityRecordRepository>();

			_uut = new StudentStatusRecordTransformer(_students.Object, _activities.Object);
		}

		[Test]
		public void TransformCreatesNewStudentRecordIfNotExists()
		{
			var studentId = 3;
			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] {
				new StudentStatusRecord{
					StudentId = studentId,
					StudentEnrollmentDate = enrollDate,
					BatchHash = batchHash,
				},
			};

			_students.Setup(s => s.GetByPACyberId(studentId)).Returns((Student)null);

			var count = 7;
			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(count)); // TODO(Erik): all the change records
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.NEW_STUDENT));
			Assert.That(actual[0].PACyberId, Is.EqualTo(studentId));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));

			_activities.Verify(
				a => a.Create(It.Is<StudentActivityRecord>(r => r.PACyberId == studentId)),
				Times.Exactly(count));
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

		private static Student NewStudent(int paCyberId) => new Student
		{
			PACyberId = paCyberId,
			PASecuredId = null,
			FirstName = "First",
			MiddleInitial = "M",
			LastName = "Last",
			Grade = "0",
			DateOfBirth = DateTime.MinValue,
			Street1 = "Street1",
			Street2 = "Street2",
			City = "City",
			State = "State",
			ZipCode = "Zip",
			IsSpecialEducation = false,
			CurrentIep = null,
			FormerIep = null,
			NorepDate = null,
			StartDate = DateTime.MinValue,
			EndDate = null,
			SchoolDistrict = new SchoolDistrict
			{
				Aun = 123456789,
				Name = "Some SD",
			},
		};

		private static StudentStatusRecord NewStatusRecord(int paCyberId, DateTime enrollDate, string batchHash, Student student)
			=> new StudentStatusRecord
			{
				StudentId = paCyberId,
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
		[TestCase(StudentActivity.DATEOFBIRTH_CHANGE, "DateOfBirth", "2000/01/01", "StudentDateOfBirth", "2000/02/02")]
		[TestCase(StudentActivity.GRADE_CHANGE, "Grade", "11", "StudentGradeLevel", "12")]
		[TestCase(StudentActivity.NOREP_CHANGE, "NorepDate", "2018/01/01", "StudentNorep", "2018/02/02")]
		[TestCase(StudentActivity.PASECURED_CHANGE, "PASecuredId", 123456789, "StudentPASecuredId", 234567890)]
		[TestCase(StudentActivity.SPECIAL_ENROLL, "IsSpecialEducation", false, "StudentIsSpecialEducation", true)]
		[TestCase(StudentActivity.CURRENTIEP_CHANGE, "CurrentIep", "2018/01/01", "StudentCurrentIep", "2018/02/02")]
		[TestCase(StudentActivity.FORMERIEP_CHANGE, "FormerIep", "2018/01/01", "StudentFormerIep", "2018/02/02")]
		public void TransformWithExistingStudentCreatesRecordIfChanged<T>(
			string activity,
			string studentProperty, T oldData,
			string recordProperty, T newData)
		{
			var paCyberId = 3;
			var student = NewStudent(paCyberId);
			Assign(student, studentProperty, oldData);
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(paCyberId, enrollDate, batchHash, student) };
			Assign(statuses[0], recordProperty, newData);

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[0].Activity, Is.EqualTo(activity));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldData.ToString()));
			Assert.That(actual[0].NextData, Is.EqualTo(newData.ToString()));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		[TestCase(StudentActivity.NAME_CHANGE, "FirstName", "Bob", "StudentFirstName", "Charlie")]
		[TestCase(StudentActivity.NAME_CHANGE, "MiddleInitial", "A", "StudentMiddleInitial", "B")]
		[TestCase(StudentActivity.NAME_CHANGE, "LastName", "Ytset", "StudentLastName", "Testy")]
		public void TransformWithExistingStudentCreatesNameChangeRecordIfChanged(
					string activity,
					string studentProperty, string oldData,
					string recordProperty, string newData)
		{
			var paCyberId = 3;
			var student = NewStudent(paCyberId);
			Assign(student, studentProperty, oldData);
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var oldName = String.Join("|", student.FirstName, student.MiddleInitial, student.LastName);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(paCyberId, enrollDate, batchHash, student) };
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
		[TestCase(StudentActivity.ADDRESS_CHANGE, "Street1", "Here", "StudentStreet1", "There")]
		[TestCase(StudentActivity.ADDRESS_CHANGE, "Street2", "Here", "StudentStreet2", "There")]
		[TestCase(StudentActivity.ADDRESS_CHANGE, "City", "East Side", "StudentCity", "West Side")]
		[TestCase(StudentActivity.ADDRESS_CHANGE, "State", "PA", "StudentState", "Not PA")]
		[TestCase(StudentActivity.ADDRESS_CHANGE, "ZipCode", "12345", "StudentZipCode", "67890")]
		public void TransformWithExistingStudentCreatesAddressChangeRecordIfChanged(
					string activity,
					string studentProperty, string oldData,
					string recordProperty, string newData)
		{
			var paCyberId = 3;
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
			var statuses = new[] { NewStatusRecord(paCyberId, enrollDate, batchHash, student) };
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
			var studentId = 3;
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
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.DISTRICT_ENROLL));
			Assert.That(actual[0].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(oldDistrict));
			Assert.That(actual[0].NextData, Is.EqualTo(newDistrict));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatesDistrictWithdrawRecordIfPresent()
		{
			var studentId = 3;
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
			Assert.That(actual[0].Activity, Is.EqualTo(StudentActivity.DISTRICT_WITHDRAW));
			Assert.That(actual[0].Timestamp, Is.EqualTo(withdrawDate));
			Assert.That(actual[0].PreviousData, Is.EqualTo(districtAun.ToString()));
			Assert.That(actual[0].NextData, Is.EqualTo(null));
			Assert.That(actual[0].BatchHash, Is.EqualTo(batchHash));
		}

		[Test]
		public void TransformCreatesSpecialWithdrawRecordIfPresent()
		{
			var paCyberId = 3;
			var student = NewStudent(paCyberId);
			student.IsSpecialEducation = true;
			_students.Setup(ss => ss.GetByPACyberId(paCyberId)).Returns(student);

			var enrollDate = DateTime.Now.Date;
			var batchHash = "hash";
			var statuses = new[] { NewStatusRecord(paCyberId, enrollDate, batchHash, student) };
			statuses[0].StudentIsSpecialEducation = false;
			statuses[0].StudentWithdrawalDate = DateTime.Now.Date;

			var actual = (_uut.Transform(statuses) as IEnumerable<StudentActivityRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(2)); // district withdrawal is first
			Assert.That(actual[1].PACyberId, Is.EqualTo(paCyberId));
			Assert.That(actual[1].Activity, Is.EqualTo(StudentActivity.SPECIAL_WITHDRAW));
			Assert.That(actual[1].Timestamp, Is.EqualTo(enrollDate));
			Assert.That(actual[1].PreviousData, Is.EqualTo(true.ToString()));
			Assert.That(actual[1].NextData, Is.EqualTo(false.ToString()));
			Assert.That(actual[1].BatchHash, Is.EqualTo(batchHash));
		}
	}
}
