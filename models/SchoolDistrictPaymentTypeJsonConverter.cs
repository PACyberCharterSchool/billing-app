using System;
using Newtonsoft.Json;

namespace models
{
	public class SchoolDistrictPaymentTypeJsonConverter : JsonConverter<SchoolDistrictPaymentType>
	{
		public override SchoolDistrictPaymentType ReadJson(JsonReader reader, Type objectType, SchoolDistrictPaymentType existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return SchoolDistrictPaymentType.FromString((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, SchoolDistrictPaymentType value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Value);
		}
	}
}
