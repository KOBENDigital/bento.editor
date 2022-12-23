using System;
using System.Collections.Generic;
using Bento.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bento.Core.JsonConverters
{
	public class StackItemSettingsConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var dictionary = new Dictionary<string, StackItemSetting>();
			var valueString = reader.Value?.ToString();

			if (string.IsNullOrWhiteSpace(valueString))
			{
				return dictionary;
			}

			var parent = JObject.Parse(valueString);

			foreach (var child in parent.Children())
			{
				var setting = JsonConvert.DeserializeObject<StackItemSetting>(child.First.ToString());

				dictionary.Add(child.Path, setting);
			}
			
			return dictionary;
		}
		public override bool CanWrite => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}