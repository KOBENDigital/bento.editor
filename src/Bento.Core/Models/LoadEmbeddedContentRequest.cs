using Newtonsoft.Json;

namespace Bento.Core.Models {
	public class LoadEmbeddedContentRequest
	{
		[JsonProperty("guid")]
		public string Guid { get; set; }
		[JsonProperty("contentTypeAlias")]
		public string ContentTypeAlias { get; set; }
		[JsonProperty("dataJson")]
		public string DataJson { get; set; }
		[JsonProperty("culture")]
		public string Culture { get; set; }
	}
}