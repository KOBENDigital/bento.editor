using System;
using System.Collections.Generic;
using Bento.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bento.Core.JsonConverters
{
	public class StackItemConverter : JsonCreationConverter<StackItem>
	{
		protected override StackItem Create(Type objectType, JObject jObject)
		{
			return new StackItem();
		}

		private bool FieldExists(string fieldName, JObject jObject)
		{
			return jObject[fieldName] != null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}