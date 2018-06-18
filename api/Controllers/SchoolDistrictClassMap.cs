using System;
using System.Globalization;
using CsvHelper.Configuration;

using models;

namespace api.Controllers
{
	public class SchoolDistrictClassMap : ClassMap<SchoolDistrict>
	{
		public SchoolDistrictClassMap()
		{
			Map(d => d.Aun).Name("AUN");
			Map(d => d.Name).Name("School District");
			Map(d => d.Rate).Index(3).
				TypeConverterOption.NumberStyles(NumberStyles.Currency);
			Map(d => d.SpecialEducationRate).Index(4).
				TypeConverterOption.NumberStyles(NumberStyles.Currency);
		}
	}
}
