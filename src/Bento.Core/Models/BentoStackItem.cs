using Bento.Core.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Bento.Core.Models
{
	public class StackItem
	{
		[JsonProperty("alias")]
		public String Alias { get; set; }

		[JsonProperty("areas")]
		public IEnumerable<Area> Areas { get; set; } = new List<Area>();

		[JsonProperty("settings")]
		public IDictionary<string, object> SettingsData { get; set; } = new Dictionary<string, object>();

		[JsonIgnore]
		public IPublishedElement Settings { get; set; }

	}

	//hack rubbish
	public class OldStackItem
	{
		[JsonProperty("alias")]
		public String Alias { get; set; }


		[JsonProperty("areas")]
		public IEnumerable<Area> Areas { get; set; } = new List<Area>();

		[JsonProperty("settings")]
		[JsonConverter(typeof(StackItemSettingsConverter))]
		public IDictionary<string, StackItemSetting> Settings { get; set; } = new Dictionary<string, StackItemSetting>();

		public T GetSettingValue<T>(string key)
		{
			//todo: do we want to try/catch this or allow it to blow up?
			if (!Settings.ContainsKey(key))
			{
				return default(T);
			}

			var value = Settings[key].Value;

			if (string.IsNullOrWhiteSpace(value?.ToString()))
			{
				return default(T);
			}

			//todo: can we use a flag to determine if the setting is complex i.f. it requires deserializing?
			var singleValueTypes = new[] { "string", "radio", "boolean" };

			if (singleValueTypes.Contains(Settings[key].Type))
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}

			var returnValue = JsonConvert.DeserializeObject<T>(value.ToString());

			return (T)Convert.ChangeType(returnValue, typeof(T));
		}
	}
}
