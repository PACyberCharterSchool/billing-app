using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

using NUnit.Framework;

using models;
using models.Tests.Util;

namespace models.Tests
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
		public void GetByAunReturnsSchoolDistrictIfExists()
		{
			var aun = 3;
			var district = new SchoolDistrict { Aun = aun };
			_context.Add(district);
			_context.SaveChanges();

			var actual = _uut.GetByAun(aun);
			Assert.That(actual, Is.EqualTo(district));
		}

		[Test]
		public void GetByAunReturnsNullIfNotExists()
		{
			var aun = 3;

			var actual = _uut.GetByAun(aun);
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

		private static void AssertSchoolDistrict(SchoolDistrict actual, SchoolDistrict district)
		{
			Assert.That(actual.Aun, Is.EqualTo(actual.Aun));
			Assert.That(actual.Name, Is.EqualTo(actual.Name));
			Assert.That(actual.Rate, Is.EqualTo(actual.Rate));
			Assert.That(actual.AlternateRate, Is.EqualTo(actual.AlternateRate));
			Assert.That(actual.SpecialEducationRate, Is.EqualTo(actual.SpecialEducationRate));
			Assert.That(actual.AlternateSpecialEducationRate, Is.EqualTo(actual.AlternateSpecialEducationRate));
			Assert.That(actual.PaymentType, Is.EqualTo(actual.PaymentType));
		}

		[Test]
		public void CreateOrUpdateWithNewObjectCreates()
		{
			var time = DateTime.Now;
			var district = new SchoolDistrict
			{
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
				SpecialEducationRate = 2.0m,
				AlternateSpecialEducationRate = 4.0m,
				PaymentType = SchoolDistrictPaymentType.Ach,
			};

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, district));
			Assert.That(result, Is.EqualTo(district));

			var actual = _context.SchoolDistricts.First(d => d.Id == result.Id);
			AssertSchoolDistrict(actual, district);
			Assert.That(actual.Created, Is.EqualTo(time));
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateOrUpdateWithSameObjectUpdates()
		{
			var time = DateTime.Now;
			var id = 3;
			var district = new SchoolDistrict
			{
				Id = id,
				Aun = 123456789,
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
				SpecialEducationRate = 2.0m,
				AlternateSpecialEducationRate = 4.0m,
				PaymentType = SchoolDistrictPaymentType.Ach,
				LastUpdated = time.AddDays(-1),
			};
			_context.Add(district);
			_context.SaveChanges();

			district.Aun = 234567890;
			district.Name = "Charlie";
			district.Rate = 2.0m;
			district.AlternateRate = 3.0m;
			district.SpecialEducationRate = 4.0m;
			district.PaymentType = SchoolDistrictPaymentType.Check;
			_context.SaveChanges(() => _uut.CreateOrUpdate(district));

			var actual = _context.SchoolDistricts.First(d => d.Id == id);
			AssertSchoolDistrict(actual, district);
			Assert.That(actual.PaymentType, Is.EqualTo(district.PaymentType));
			Assert.That(actual.LastUpdated.Date, Is.EqualTo(time.Date));
		}

		[Test]
		public void CreateOrUpdateWithDifferentObjectUpdates()
		{
			var aun = 123456789;
			var now = DateTime.Now;
			var district = new SchoolDistrict
			{
				Aun = aun,
				Name = "Bob",
				Rate = 1.0m,
				AlternateRate = null,
				SpecialEducationRate = 2.0m,
				AlternateSpecialEducationRate = 4.0m,
				PaymentType = SchoolDistrictPaymentType.Ach,
				Created = now.AddDays(-1),
				LastUpdated = now.AddDays(-1),
			};
			_context.Add(district);
			_context.SaveChanges();

			var updated = new SchoolDistrict
			{
				Aun = aun,
				Name = "Charlie",
				Rate = 2.0m,
				AlternateRate = 3.0m,
				SpecialEducationRate = 4.0m,
				PaymentType = SchoolDistrictPaymentType.Check,
			};
			_context.SaveChanges(() => _uut.CreateOrUpdate(now, updated));

			var actual = _context.SchoolDistricts.First(d => d.Id == district.Id);
			AssertSchoolDistrict(actual, updated);
			Assert.That(actual.Created, Is.EqualTo(district.Created));
			Assert.That(actual.LastUpdated, Is.EqualTo(now));
		}
	}
}
