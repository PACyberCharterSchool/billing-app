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
			_students.Setup(s => s.GetMany(null, null, 0, 0)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.That(actual, Is.EqualTo(students));
		}

		[Test]
		public async Task GetManyAllArgsReturnsList()
		{
			var field = "FirstName";
			var dir = "asc";
			var skip = 10;
			var take = 100;
			var students = new List<Student>{
				new Student(),
			};
			_students.Setup(s => s.GetMany(field, dir, skip, take)).Returns(students);

			var result = await _uut.GetMany(new StudentsController.GetManyArgs
			{
				Field = field,
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
			_students.Setup(s => s.GetMany(null, null, 0, 0)).Returns(new List<Student>());

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
		public async Task GetManyBadRequestOnBadField()
		{
			var args = new StudentsController.GetManyArgs
			{
				Field = "bob",
			};
			var result = await _uut.GetMany(args);

			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var error = ((ErrorResponse)value).Error;
			Assert.That(error, Is.EqualTo("Invalid sort field 'bob'."));
		}

		[Test]
		public async Task GetManyBadRequestOnBadDir()
		{
			var args = new StudentsController.GetManyArgs
			{
				Dir = "bob",
			};
			var result = await _uut.GetMany(args);

			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var error = ((ErrorResponse)value).Error;
			Assert.That(error, Is.EqualTo("Sort direction must be 'asc' or 'desc'; was 'bob'."));
		}

		[Test]
		public async Task GetManyBadRequestOnBadSkip()
		{
			var args = new StudentsController.GetManyArgs
			{
				Skip = -1,
			};
			var result = await _uut.GetMany(args);

			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var error = ((ErrorResponse)value).Error;
			Assert.That(error, Is.EqualTo("Skip must be 0 or greater; was '-1'."));
		}

		[Test]
		public async Task GetManyBadRequestOnBadTake()
		{
			var args = new StudentsController.GetManyArgs
			{
				Take = -1,
			};
			var result = await _uut.GetMany(args);

			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var error = ((ErrorResponse)value).Error;
			Assert.That(error, Is.EqualTo("Take must be 0 or greater; was '-1'."));
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
