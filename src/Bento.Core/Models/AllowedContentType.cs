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
	}
}
