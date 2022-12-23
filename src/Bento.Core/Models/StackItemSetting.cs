using Newtonsoft.Json;

namespace Bento.Core.Models
{
	public class StackItemSetting
	{
		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("value")]
		public object Value { get; set; }
	}
}