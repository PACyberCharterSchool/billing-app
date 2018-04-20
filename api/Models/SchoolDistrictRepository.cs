using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface ISchoolDistrictRepository
	{
		SchoolDistrict Get(int id);
		IList<SchoolDistrict> GetMany();
	}

	public class SchoolDistrictRepository : ISchoolDistrictRepository
	{
		private readonly DbSet<SchoolDistrict> _schoolDistricts;
		private readonly ILogger<SchoolDistrictRepository> _logger;

		public SchoolDistrictRepository(PacBillContext context, ILogger<SchoolDistrictRepository> logger)
		{
			_schoolDistricts = context.SchoolDistricts;
			_logger = logger;
		}

		public SchoolDistrict Get(int id) => _schoolDistricts.SingleOrDefault(d => d.Id == id);

		public IList<SchoolDistrict> GetMany() => _schoolDistricts.ToList();
	}
}
