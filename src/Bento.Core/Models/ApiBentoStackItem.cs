using Newtonsoft.Json;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Bento.Core.Models
{
	public class ApiStackItem
	{
		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("areas")]
		public IEnumerable<ApiArea> Areas { get; set; } = new List<ApiArea>();

		[JsonProperty("settings")]
		public IApiElement? Settings { get; set; }
	}
}