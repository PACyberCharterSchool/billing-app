using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

using Moq;
using NUnit.Framework;

using api.Models;
using api.Tests.Util;

namespace api.Tests.Models
{
	[TestFixture]
	public class StudentRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<StudentRepository> _logger;

		private StudentRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("students").Options);
			_logger = new TestLogger<StudentRepository>();

			_uut = new StudentRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			// in-memory database sticks around
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void GetReturnsStudentIfExists()
		{
			var id = 3;
			var student = new Student
			{
				Id = 3,
			};
			_context.Add(student);
			_context.SaveChanges();

			var actual = _uut.Get(id);
			Assert.That(actual, Is.EqualTo(student));
		}

		[Test]
		public void GetReturnsNullIfNotExists()
		{
			var actual = _uut.Get(3);
			Assert.That(actual, Is.Null);
		}

		[Test]
		public void GetManyWithDefaultValuesGetsEverythingSortedByIdAsc()
		{
			var students = new[] {
				new Student{Id = 3},
				new Student{Id = 2},
				new Student{Id = 1},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var actual = _uut.GetMany();
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(1));
			Assert.That(actual[1].Id, Is.EqualTo(2));
			Assert.That(actual[2].Id, Is.EqualTo(3));
		}

		[Test]
		public void GetManyWithFieldSortsByField()
		{
			var students = new[] {
				new Student{Id = 1, FirstName = "C"},
				new Student{Id = 2, FirstName = "B"},
				new Student{Id = 3, FirstName = "A"},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var actual = _uut.GetMany("FirstName");
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(3));
			Assert.That(actual[1].Id, Is.EqualTo(2));
			Assert.That(actual[2].Id, Is.EqualTo(1));
		}

		[Test]
		public void GetManyWithBadFieldThrows()
		{
			Assert.That(() => _uut.GetMany("bad"), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void GetManyWithDescSortsDesc()
		{
			var students = new[] {
				new Student{Id = 1},
				new Student{Id = 2},
				new Student{Id = 3},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var actual = _uut.GetMany(null, "desc");
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(3));
			Assert.That(actual[1].Id, Is.EqualTo(2));
			Assert.That(actual[2].Id, Is.EqualTo(1));
		}

		[Test]
		public void GetManyWithBadDirThrows()
		{
			Assert.That(() => _uut.GetMany("Id", "bad"), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void GetManyWithSkipNotZeroSkips()
		{
			var students = new[] {
				new Student{Id = 1},
				new Student{Id = 2},
				new Student{Id = 3},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var actual = _uut.GetMany(1, 0);
			Assert.That(actual, Has.Count.EqualTo(2));
			Assert.That(actual[0].Id, Is.EqualTo(2));
			Assert.That(actual[1].Id, Is.EqualTo(3));
		}

		[Test]
		public void GetManyWithSkipLessThanZeroThrows()
		{
			Assert.That(() => _uut.GetMany(-1, 0), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void GetManyWithTakeNotZeroTakes()
		{
			var students = new[] {
				new Student{Id = 1},
				new Student{Id = 2},
				new Student{Id = 3},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var actual = _uut.GetMany(0, 1);
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].Id, Is.EqualTo(1));
		}

		[Test]
		public void GetManyWithTakeLessThanZeroThrows()
		{
			Assert.That(() => _uut.GetMany(0, -1), Throws.TypeOf<ArgumentException>());
		}
	}
}
