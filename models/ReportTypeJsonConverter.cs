using System;
using Newtonsoft.Json;

namespace models
{
	public class ReportTypeJsonConverter : JsonConverter<ReportType>
	{
		public override ReportType ReadJson(JsonReader reader, Type objectType, ReportType existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return ReportType.FromString((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, ReportType value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Value);
		}
	}
}
