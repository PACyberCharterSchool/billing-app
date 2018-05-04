using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using static models.Common.PropertyMerger;

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

		private IList<string> _excludedFields = new List<string>
		{
			nameof(SchoolDistrict.Id),
			nameof(SchoolDistrict.Created),
			nameof(SchoolDistrict.LastUpdated),
		};

		public SchoolDistrict CreateOrUpdate(DateTime time, SchoolDistrict update)
		{
			var district = _schoolDistricts.FirstOrDefault(d => d.Id == update.Id);
			if (district == null)
			{
				update.Created = time;
				update.LastUpdated = time;

				_schoolDistricts.Add(update);
				return update;
			}

			MergeProperties(district, update, _excludedFields);
			district.LastUpdated = time;
			_schoolDistricts.Update(district);

			return district;
		}

		public SchoolDistrict CreateOrUpdate(SchoolDistrict update) => CreateOrUpdate(DateTime.Now, update);
	}
}
