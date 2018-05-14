using System;
using Newtonsoft.Json;

using models;

namespace api.Dtos
{
	public class PaymentTypeJsonConverter : JsonConverter<PaymentType>
	{
		public override PaymentType ReadJson(JsonReader reader, Type objectType, PaymentType existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return PaymentType.FromString((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, PaymentType value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Value);
		}
	}
}
