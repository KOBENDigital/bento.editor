using System;
using Newtonsoft.Json;

namespace Bento.Core.Models
{
	public class RelationItem
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("key")]
		public Guid Key { get; set; }

		[JsonProperty("icon")]
		public string Icon { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("status")]
		public bool Status { get; set; }

		[JsonProperty("lastEdited")]
		public DateTime LastEdited { get; set; }

		[JsonProperty("createdBy")]
		public string CreatedBy { get; set; }
	}
}