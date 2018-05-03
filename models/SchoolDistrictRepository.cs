using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface ISchoolDistrictRepository
	{
		SchoolDistrict CreateOrUpdate(SchoolDistrict district);
		SchoolDistrict CreateOrUpdate(DateTime time, SchoolDistrict district);
		SchoolDistrict Get(int id);
		SchoolDistrict GetByAun(int aun);
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

		public SchoolDistrict GetByAun(int aun) => _schoolDistricts.SingleOrDefault(d => d.Aun == aun);

		public IList<SchoolDistrict> GetMany() => _schoolDistricts.OrderBy(d => d.Id).ToList();

		public SchoolDistrict CreateOrUpdate(SchoolDistrict update) => CreateOrUpdate(DateTime.Now, update);

		public SchoolDistrict CreateOrUpdate(DateTime time, SchoolDistrict update)
		{
			var district = _schoolDistricts.FirstOrDefault(d => d.Id == update.Id);
			if (district == null)
			{
				update.Created = time;
				update.LastUpdated = time;

				_schoolDistricts.Add(update);
				_context.SaveChanges();
				return update;
			}

			if (district.Aun != update.Aun)
				district.Aun = update.Aun;

			if (district.Name != update.Name)
				district.Name = update.Name;

			if (district.Rate != update.Rate)
				district.Rate = update.Rate;

			if (district.AlternateRate != update.AlternateRate)
				district.AlternateRate = update.AlternateRate;

			if (district.PaymentType != update.PaymentType)
				district.PaymentType = update.PaymentType;

			district.LastUpdated = time;
			_schoolDistricts.Update(district);
			_context.SaveChanges();

			return district;
		}
	}
}
