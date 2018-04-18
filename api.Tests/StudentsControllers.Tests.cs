using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		public async Task GetAllReturnsList()
		{
			var students = new List<Student>{
				new Student(),
				new Student(),
			};
			_students.Setup(s => s.GetMany()).Returns(students);

			var result = await _uut.GetAll();
			Assert.That(result, Is.TypeOf(typeof(ObjectResult)));
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.That(actual, Is.EqualTo(students));
		}

		[Test]
		public async Task GetAllReturnsEmptyListWhenEmpty()
		{
			_students.Setup(s => s.GetMany()).Returns(new List<Student>());

			var result = await _uut.GetAll();
			Assert.That(result, Is.TypeOf(typeof(ObjectResult)));
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.Zero(actual.Count);
		}

		[Test]
		public async Task GetAllReturnsEmptyListWhenNull()
		{
			_students.Setup(s => s.GetMany()).Returns((List<Student>)null);

			var result = await _uut.GetAll();
			Assert.That(result, Is.TypeOf(typeof(ObjectResult)));
			var value = ((ObjectResult)result).Value;

			var actual = ((StudentsController.StudentsResponse)value).Students;
			Assert.Zero(actual.Count);
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
			Assert.That(result, Is.TypeOf(typeof(ObjectResult)));
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
			Assert.That(result, Is.TypeOf(typeof(NotFoundResult)));
		}
	}
}
