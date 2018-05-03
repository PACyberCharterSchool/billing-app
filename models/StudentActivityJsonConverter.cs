using System;
using Newtonsoft.Json;

namespace models
{
	public class StudentActivityJsonConverter : JsonConverter<StudentActivity>
	{
		public override StudentActivity ReadJson(JsonReader reader, Type objectType, StudentActivity existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return StudentActivity.FromString((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, StudentActivity value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Value);
		}
	}
}
