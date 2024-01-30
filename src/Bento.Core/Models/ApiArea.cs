using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Bento.Core.JsonConverters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models.DeliveryApi;

namespace Bento.Core.Models
{
	public class ApiArea
	{
		[JsonConverter(typeof(NullToDefaultConverter<Guid>))]
		[JsonProperty("key")]
		public Guid Key { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("content")]
		public IApiElement Content { get; set; }
	}
} 