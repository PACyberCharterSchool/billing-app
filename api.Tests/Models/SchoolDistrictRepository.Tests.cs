using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

using NUnit.Framework;

using api.Models;
using api.Tests.Util;

namespace api.Tests.Models
{
	[TestFixture]
	public class SchoolDistrictRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<SchoolDistrictRepository> _logger;

		private SchoolDistrictRepository _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("school_districts").Options);
			_logger = new TestLogger<SchoolDistrictRepository>();

			_uut = new SchoolDistrictRepository(_context, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			// in-memory database sticks around
			_context.Database.EnsureDeleted();
		}

		[Test]
		public void GetReturnsSchoolDistrictIfExists()
		{
			var id = 3;
			var district = new SchoolDistrict { Id = id };
			_context.Add(district);
			_context.SaveChanges();

			var actual = _uut.Get(id);
			Assert.That(actual, Is.EqualTo(district));
		}

		[Test]
		public void GetReturnsNullIfNotExists()
		{
			var id = 3;

			var actual = _uut.Get(id);
			Assert.IsNull(actual);
		}

		[Test]
		public void GetManyReturnsAll()
		{
			var districts = new[] {
				new SchoolDistrict{Id = 1},
				new SchoolDistrict{Id = 2},
				new SchoolDistrict{Id = 3},
			};
			_context.AddRange(districts);
			_context.SaveChanges();

			var actual = _uut.GetMany();
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual, Is.EqualTo(districts));
		}

		[Test]
		public void GetManyReturnsAllOrderedById()
		{
			var districts = new[] {
				new SchoolDistrict{Id = 3, Name = "B"},
				new SchoolDistrict{Id = 1, Name = "C"},
				new SchoolDistrict{Id = 2, Name = "A"},
			};
			_context.AddRange(districts);
			_context.SaveChanges();

			var actual = _uut.GetMany();
			Assert.That(actual, Has.Count.EqualTo(3));
			Assert.That(actual[0].Id, Is.EqualTo(districts[1].Id));
			Assert.That(actual[1].Id, Is.EqualTo(districts[2].Id));
			Assert.That(actual[2].Id, Is.EqualTo(districts[0].Id));
		}

		[Test]
		public void CreateOrUpdateWithNewObjectCreates()
		{
			var district = new SchoolDistrict
			{
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
			};

			var result = _uut.CreateOrUpdate(district);
			Assert.That(result, Is.EqualTo(district));

			var actual = _context.SchoolDistricts.First(d => d.Id == result.Id);
			Assert.That(actual.Name, Is.EqualTo(district.Name));
			Assert.That(actual.Rate, Is.EqualTo(district.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(district.AlternateRate));
		}

		[Test]
		public void CreateOrUpdateWithSameObjectUpdates()
		{
			var id = 3;
			var district = new SchoolDistrict
			{
				Id = id,
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
			};
			_context.Add(district);
			_context.SaveChanges();


			district.Name = "Charlie";
			district.Rate = 2.0m;
			district.AlternateRate = 3.0m;
			_uut.CreateOrUpdate(district);

			var actual = _context.SchoolDistricts.First(d => d.Id == id);
			Assert.That(actual.Name, Is.EqualTo(district.Name));
			Assert.That(actual.Rate, Is.EqualTo(district.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(district.AlternateRate));
		}

		[Test]
		public void CreateOrUpdateWithDifferentObjectUpdates()
		{
			var id = 3;
			var district = new SchoolDistrict
			{
				Id = id,
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
			};
			_context.Add(district);
			_context.SaveChanges();

			var updated = new SchoolDistrict
			{
				Id = id,
				Name = "Charlie",
				Rate = 2.0m,
				AlternateRate = 3.0m,
			};
			_uut.CreateOrUpdate(updated);

			var actual = _context.SchoolDistricts.First(d => d.Id == id);
			Assert.That(actual.Name, Is.EqualTo(updated.Name));
			Assert.That(actual.Rate, Is.EqualTo(updated.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(updated.AlternateRate));
		}
	}
}
