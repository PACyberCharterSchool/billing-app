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
		IList<SchoolDistrict> CreateOrUpdateMany(DateTime time, IList<SchoolDistrict> districts);
		IList<SchoolDistrict> CreateOrUpdateMany(IList<SchoolDistrict> districts);
		SchoolDistrict CreateOrUpdate(DateTime time, SchoolDistrict district);
		SchoolDistrict CreateOrUpdate(SchoolDistrict district);
		SchoolDistrict Get(int id);
		SchoolDistrict GetByAun(int aun);
		IList<SchoolDistrict> GetMany();
		IList<int> GetManyAuns();
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

		public IList<int> GetManyAuns() => _schoolDistricts.OrderBy(d => d.Aun).Select(d => d.Aun).ToList();

		private IList<string> _excludedFields = new List<string>
		{
			nameof(SchoolDistrict.Id),
			nameof(SchoolDistrict.Aun),
			nameof(SchoolDistrict.Created),
			nameof(SchoolDistrict.LastUpdated),
		};

		public IList<SchoolDistrict> CreateOrUpdateMany(DateTime time, IList<SchoolDistrict> updates)
		{
			var districts = _schoolDistricts.Where(d => updates.Select(u => u.Aun).Contains(d.Aun));
			var added = new List<SchoolDistrict>();
			var updated = new List<SchoolDistrict>();

			foreach (var update in updates)
			{
				var district = districts.FirstOrDefault(d => d.Aun == update.Aun);
				if (district == null)
				{
					update.Created = time;
					update.LastUpdated = time;

					added.Add(update);
					continue;
				}

				MergeProperties(district, update, _excludedFields);
				district.LastUpdated = time;
				updated.Add(district);
			}

			_schoolDistricts.AddRange(added);
			_schoolDistricts.UpdateRange(updated);

			updated.AddRange(added);
			return updated.OrderBy(u => u.Id).ToList();
		}

		public IList<SchoolDistrict> CreateOrUpdateMany(IList<SchoolDistrict> updates) =>
			CreateOrUpdateMany(DateTime.Now, updates);

		public SchoolDistrict CreateOrUpdate(DateTime time, SchoolDistrict update) =>
			CreateOrUpdateMany(time, new[] { update })[0];

		public SchoolDistrict CreateOrUpdate(SchoolDistrict update) => CreateOrUpdate(DateTime.Now, update);
	}
}
