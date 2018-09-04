using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using models;

namespace api.Controllers
{
	public class SchoolDistrictClassMap : ClassMap<SchoolDistrict>
	{
		private class SchoolDistrictPaymentTypeConverter : ITypeConverter
		{
			public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
				=> string.IsNullOrWhiteSpace(text) ? null : SchoolDistrictPaymentType.FromString(text);

			public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
				=> ((SchoolDistrictPaymentType)value).Value;
		}

		public SchoolDistrictClassMap()
		{
			Map(d => d.Aun).Name("AUN");
			Map(d => d.Name).Name("School District");
			Map(d => d.Rate).Index(2).
				TypeConverterOption.NumberStyles(NumberStyles.Currency);
			Map(d => d.SpecialEducationRate).Index(3).
				TypeConverterOption.NumberStyles(NumberStyles.Currency);
			Map(d => d.PaymentType).Index(4).TypeConverter<SchoolDistrictPaymentTypeConverter>();
		}
	}
}
