using Newtonsoft.Json;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Bento.Core.Models
{
	public class StackItem
	{
		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("areas")]
		public IEnumerable<Area> Areas { get; set; } = new List<Area>();

		[JsonProperty("settings")]
		public IDictionary<string, object> SettingsData { get; set; } = new Dictionary<string, object>();

		[JsonIgnore]
		public IPublishedElement Settings { get; set; }
	}
}