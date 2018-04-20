using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
	}
}
