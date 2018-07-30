using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;
using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
{
	[TestFixture]
	public class StudentRepositoryTests
	{
		private PacBillContext _context;
		private Mock<IFilterParser> _parser;
		private ILogger<StudentRepository> _logger;

		private StudentRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("students").Options);
			_parser = new Mock<IFilterParser>();
			_logger = new TestLogger<StudentRepository>();

			_uut = new StudentRepository(_context, _parser.Object, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			// in-memory database sticks around
			_context.Database.EnsureDeleted();
		}

		private Student NewStudent(DateTime time) => new Student
		{
			Id = 1,
			PACyberId = "3",
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
			Created = DateTime.MinValue,
			LastUpdated = DateTime.MinValue,
			SchoolDistrict = new SchoolDistrict
			{
				Aun = 123456789,
				Name = "Some SD",
			},
		};

		[Test]
		public void CreateOrUpdateWithNewObjectCreates()
		{
			var time = DateTime.Now;
			var student = NewStudent(time);

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, student));
			Assert.That(result, Is.EqualTo(student));

			var actual = _context.Students.First(s => s.Id == result.Id);
			Assert.That(actual, Is.EqualTo(student));
			Assert.That(actual.Created, Is.EqualTo(time));
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateOrUpdateWithSameObjectUpdates()
		{
			var time = DateTime.Now;
			var id = 3;
			var student = NewStudent(time);
			student.Id = 3;
			_context.Add(student);
			_context.SaveChanges();

			student.FirstName = "Updated";
			_context.SaveChanges(() => _uut.CreateOrUpdate(student));

			var actual = _context.Students.First(s => s.Id == id);
			Assert.That(actual.FirstName, Is.EqualTo(student.FirstName));
			Assert.That(actual.Created, Is.EqualTo(DateTime.MinValue));
			Assert.That(actual.LastUpdated.ToString(), Is.EqualTo(time.ToString()));
		}

		[Test]
		public void CreateOrUpdateWithDifferentObjectUpdates()
		{
			var time = DateTime.Now;

			var id = 3;
			var student = NewStudent(time);
			student.Id = id;
			_context.Add(student);
			_context.SaveChanges();

			var updated = NewStudent(time);
			updated.Id = id;
			updated.FirstName = "Updated";
			_context.SaveChanges(() => _uut.CreateOrUpdate(time, updated));

			var actual = _context.Students.First(s => s.Id == id);
			Assert.That(actual.FirstName, Is.EqualTo(student.FirstName));
			Assert.That(actual.Created, Is.EqualTo(DateTime.MinValue));
			Assert.That(actual.LastUpdated.ToString(), Is.EqualTo(time.ToString()));
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
		public void GetByPACyberIdReturnsStudentIfExists()
		{
			var id = "3";
			var student = new Student
			{
				PACyberId = id,
			};
			_context.Add(student);
			_context.SaveChanges();

			var actual = _uut.GetByPACyberId(id);
			Assert.That(actual, Is.EqualTo(student));
		}

		[Test]
		public void GetByPACyberIdReturnsNullIfNotExists()
		{
			var actual = _uut.GetByPACyberId("3");
			Assert.IsNull(actual);
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

			var actual = _uut.GetMany(sort: "FirstName");
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(3));
			Assert.That(actual[1].Id, Is.EqualTo(2));
			Assert.That(actual[2].Id, Is.EqualTo(1));
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

			var actual = _uut.GetMany(dir: SortDirection.Descending);
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(3));
			Assert.That(actual[1].Id, Is.EqualTo(2));
			Assert.That(actual[2].Id, Is.EqualTo(1));
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

			var actual = _uut.GetMany(skip: 1);
			Assert.That(actual, Has.Count.EqualTo(2));
			Assert.That(actual[0].Id, Is.EqualTo(2));
			Assert.That(actual[1].Id, Is.EqualTo(3));
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

			var actual = _uut.GetMany(take: 1);
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].Id, Is.EqualTo(1));
		}

		[Test]
		public void GetManyWithFilterFilters()
		{
			var students = new[] {
				new Student{Id = 1, FirstName = "Alice"},
				new Student{Id = 2, FirstName = "Bob"},
				new Student{Id = 3, FirstName = "Charlie"},
			};
			_context.AddRange(students);
			_context.SaveChanges();

			var filter = "(FirstName eq Bob)";
			var param = Expression.Parameter(typeof(Student), "x");
			var field = Expression.PropertyOrField(param, "FirstName");
			var method = Expression.Equal(field, Expression.Constant("Bob"));
			var exp = Expression.Lambda<Func<Student, bool>>(method, param);
			_parser.Setup(p => p.Parse<Student>("x", filter)).Returns(exp);

			var actual = _uut.GetMany(filter: filter);
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].FirstName, Is.EqualTo(students[1].FirstName));
		}
	}
}
