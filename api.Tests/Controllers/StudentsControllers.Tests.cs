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

		private static void AssertStudent(Student student, object actual)
		{
			foreach (var p in typeof(Student).GetProperties())
			{
				if (p.Name == nameof(Student.SchoolDistrict))
					continue;

				Assert.That(actual.GetType().GetField(p.Name).GetValue(actual), Is.EqualTo(p.GetValue(student)));
			}

			var district = actual.GetType().GetField(nameof(Student.SchoolDistrict)).GetValue(actual);
			foreach (var p in typeof(SchoolDistrict).GetProperties())
			{
				if (p.Name == nameof(SchoolDistrict.Students))
					continue;

				Assert.That(district.GetType().GetField(p.Name).GetValue(district),
					Is.EqualTo(p.GetValue(student.SchoolDistrict)));
			}

			Assert.That(district.GetType().GetField("Students"), Is.Null);
		}

		[Test]
		public async Task GetManyNoArgsReturnsList()
		{
			var students = new List<Student>{
				new Student
				{
					SchoolDistrict = new SchoolDistrict(),
				},
			};
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actuals = value.GetType().GetProperty("Students").GetValue(value);
			var actual = ((IList)actuals)[0];
			AssertStudent(students[0], actual);
		}

		[Test]
		public async Task GetManyAllArgsReturnsList()
		{
			var sort = "FirstName";
			var dir = SortDirection.Ascending;
			var skip = 10;
			var take = 100;
			var students = new List<Student>{
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

			var actuals = value.GetType().GetProperty("Students").GetValue(value);
			var actual = ((IList)actuals)[0];
			AssertStudent(students[0], actual);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(new List<Student>());

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = value.GetType().GetProperty("Students").GetValue(value);
			Assert.Zero((int)actual.GetType().GetProperty("Count").GetValue(actual));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns((List<Student>)null);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = value.GetType().GetProperty("Students").GetValue(value);
			Assert.Zero((int)actual.GetType().GetProperty("Count").GetValue(actual));
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

			var actual = value.GetType().GetProperty("Student").GetValue(value);
			AssertStudent(student, actual);
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
