using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Models;
using api.Tests.Util;

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

		[Test]
		public async Task GetManyNoArgsReturnsList()
		{
			var students = new List<Student>{
				new Student(),
			};
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.That(actual, Is.EqualTo(students));
		}

		[Test]
		public async Task GetManyAllArgsReturnsList()
		{
			var sort = "FirstName";
			var dir = "asc";
			var skip = 10;
			var take = 100;
			var students = new List<Student>{
				new Student(),
			};
			_students.Setup(s => s.GetMany(sort, dir, skip, take, null)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs
			{
				Sort = sort,
				Dir = dir,
				Skip = skip,
				Take = take,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.That(actual, Is.EqualTo(students));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0, null)).Returns(new List<Student>());

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.Zero(actual.Count);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_students.Setup(s => s.GetMany(null, null, 0, 0)).Returns((List<Student>)null);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.Zero(actual.Count);
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
			var student = new Student
			{
				Id = id,
			};
			_students.Setup(s => s.Get(id)).Returns(student);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentResponse)value).Student;
			Assert.That(actual, Is.EqualTo(student));
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
