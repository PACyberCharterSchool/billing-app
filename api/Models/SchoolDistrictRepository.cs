using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface ISchoolDistrictRepository
	{
		SchoolDistrict CreateOrUpdate(SchoolDistrict district);
		SchoolDistrict Get(int id);
		IList<SchoolDistrict> GetMany();
	}

	public class SchoolDistrictRepository : ISchoolDistrictRepository
	{
		private readonly PacBillContext _context;
		private readonly DbSet<SchoolDistrict> _schoolDistricts;
		private readonly ILogger<SchoolDistrictRepository> _logger;

		public SchoolDistrictRepository(PacBillContext context, ILogger<SchoolDistrictRepository> logger)
		{
			_context = context;
			_schoolDistricts = context.SchoolDistricts;
			_logger = logger;
		}

		public SchoolDistrict Get(int id) => _schoolDistricts.SingleOrDefault(d => d.Id == id);

		public IList<SchoolDistrict> GetMany() => _schoolDistricts.OrderBy(d => d.Id).ToList();

		public SchoolDistrict CreateOrUpdate(SchoolDistrict update)
		{
			var district = _schoolDistricts.FirstOrDefault(d => d.Id == update.Id);
			if (district == null)
			{
				_schoolDistricts.Add(update);
				_context.SaveChanges();
				return update;
			}

			if (district.Name != update.Name)
				district.Name = update.Name;

			if (district.Rate != update.Rate)
				district.Rate = update.Rate;

			if (district.AlternateRate != update.AlternateRate)
				district.AlternateRate = update.AlternateRate;

			_schoolDistricts.Update(district);
			_context.SaveChanges();

			return district;
		}
	}
}
