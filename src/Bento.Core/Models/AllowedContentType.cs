using Newtonsoft.Json;

namespace Bento.Core.Models
{
	public class AllowedContentType
	{
		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
		[JsonProperty("icon")]
		public string Icon { get; set; }

		[JsonProperty("preview")]
		public string Preview { get; set; }
	}
}
