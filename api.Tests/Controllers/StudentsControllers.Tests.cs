using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Dtos;
using api.Tests.Util;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class StudentsControllerTests
	{
		private Mock<IStudentRepository> _students;
		private ILogger<StudentsController> _logger;

		private StudentsController _uut;

		[SetUp]
		public void SetUp()
		{
			_students = new Mock<IStudentRepository>();
			_logger = new TestLogger<StudentsController>();

			_uut = new StudentsController(_students.Object, _logger);
		}

		public void AssertSchoolDistrict(SchoolDistrictDto actual, SchoolDistrict district)
		{
			Assert.That(actual.Id, Is.EqualTo(district.Id));
			Assert.That(actual.Aun, Is.EqualTo(district.Aun));
			Assert.That(actual.Name, Is.EqualTo(district.Name));
			Assert.That(actual.Rate, Is.EqualTo(district.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(district.AlternateRate));
			Assert.That(actual.PaymentType, Is.EqualTo(district.PaymentType));
			Assert.That(actual.Created, Is.EqualTo(district.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(district.LastUpdated));
		}

		public void AssertStudent(StudentDto actual, Student student)
		{
			Assert.That(actual.Id, Is.EqualTo(student.Id));
			Assert.That(actual.PACyberId, Is.EqualTo(student.PACyberId));
			Assert.That(actual.PASecuredId, Is.EqualTo(student.PASecuredId));
			Assert.That(actual.FirstName, Is.EqualTo(student.FirstName));
			Assert.That(actual.MiddleInitial, Is.EqualTo(student.MiddleInitial));
			Assert.That(actual.LastName, Is.EqualTo(student.LastName));
			Assert.That(actual.Grade, Is.EqualTo(student.Grade));
			Assert.That(actual.DateOfBirth, Is.EqualTo(student.DateOfBirth));
			Assert.That(actual.Street1, Is.EqualTo(student.Street1));
			Assert.That(actual.Street2, Is.EqualTo(student.Street2));
			Assert.That(actual.City, Is.EqualTo(student.City));
			Assert.That(actual.State, Is.EqualTo(student.State));
			Assert.That(actual.ZipCode, Is.EqualTo(student.ZipCode));
			Assert.That(actual.IsSpecialEducation, Is.EqualTo(student.IsSpecialEducation));
			Assert.That(actual.CurrentIep, Is.EqualTo(student.CurrentIep));
			Assert.That(actual.FormerIep, Is.EqualTo(student.FormerIep));
			Assert.That(actual.NorepDate, Is.EqualTo(student.NorepDate));
			Assert.That(actual.StartDate, Is.EqualTo(student.StartDate));
			Assert.That(actual.EndDate, Is.EqualTo(student.EndDate));
			Assert.That(actual.Created, Is.EqualTo(student.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(student.LastUpdated));

			AssertSchoolDistrict(actual.SchoolDistrict, student.SchoolDistrict);
		}

		[Test]
		public async Task GetManyNoArgsReturnsList()
		{
			var students = new[] {
				new Student
				{
					SchoolDistrict = new SchoolDistrict(),
				},
			};
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<StudentsController.StudentsResponse>());
			var actual = ((StudentsController.StudentsResponse)value).Students;

			Assert.That(actual, Has.Count.EqualTo(students.Length));
			for (var i = 0; i < actual.Count; i++)
				AssertStudent(actual[i], students[i]);
		}

		[Test]
		public async Task GetManyAllArgsReturnsList()
		{
			var sort = "FirstName";
			var dir = SortDirection.Ascending;
			var skip = 10;
			var take = 100;
			var students = new[] {
				new Student
				{
					SchoolDistrict = new SchoolDistrict(),
				},
			};
			_students.Setup(s => s.GetMany(sort, dir, skip, take, null)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs
			{
				Sort = sort,
				Dir = dir.Value,
				Skip = skip,
				Take = take,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<StudentsController.StudentsResponse>());
			var actual = ((StudentsController.StudentsResponse)value).Students;

			Assert.That(actual, Has.Count.EqualTo(students.Length));
			for (var i = 0; i < actual.Count; i++)
				AssertStudent(actual[i], students[i]);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(new List<Student>());

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<StudentsController.StudentsResponse>());
			var actual = ((StudentsController.StudentsResponse)value).Students;

			Assert.That(actual, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns((List<Student>)null);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<StudentsController.StudentsResponse>());
			var actual = ((StudentsController.StudentsResponse)value).Students;

			Assert.That(actual, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyBadRequestWhenModelStateInvalid()
		{
			var key = "error key";
			var msg = "error msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var errors = ((ErrorsResponse)value).Errors;
			Assert.That(errors, Has.Count.EqualTo(1));
			Assert.That(errors[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task GetByIdReturnsStudent()
		{
			var id = 3;
			var aun = 123456789;
			var student = new Student
			{
				Id = id,
				SchoolDistrict = new SchoolDistrict
				{
					Aun = aun,
				}
			};
			_students.Setup(s => s.Get(id)).Returns(student);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<StudentsController.StudentResponse>());
			var actual = ((StudentsController.StudentResponse)value).Student;

			AssertStudent(actual, student);
		}

		[Test]
		public async Task GetByIdReturnNotFound()
		{
			var id = 3;
			_students.Setup(s => s.Get(id)).Returns((Student)null);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}
	}
}
