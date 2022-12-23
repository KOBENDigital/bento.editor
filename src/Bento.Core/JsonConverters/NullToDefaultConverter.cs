//taken from https://stackoverflow.com/questions/31747712/json-net-deserialization-null-guid-case
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bento.Core.JsonConverters
{
	public class NullToDefaultConverter<T> : JsonConverter where T : struct
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(T);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			if (token == null || token.Type == JTokenType.Null)
			{
				return default(T);
			}
			return token.ToObject(objectType);
		}

		// Return false instead if you don't want default values to be written as null
		public override bool CanWrite => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (EqualityComparer<T>.Default.Equals((T)value, default(T)))
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value);
			}
		}
	}
}