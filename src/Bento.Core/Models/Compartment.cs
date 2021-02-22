using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Bento.Core.JsonConverters;
using Umbraco.Core.Models.PublishedContent;

namespace Bento.Core.Models
{
	public class Area
	{
		[JsonProperty("contents")]
		public List<AreaItem> Contents { get; set; }

		[JsonProperty("settings")]
		public IDictionary<string, object> SettingsData { get; set; } = new Dictionary<string, object>();

		[JsonIgnore]
		public IPublishedElement Settings { get; set; }

		/**
		 * Old "Single Item" Area setup (check if any of these are set, make key/id nullable, and then assume it is not Multi)
		**/

		[JsonConverter(typeof(NullToDefaultConverter<Guid>))]
		[JsonProperty("key")]
		public Guid? Key { get; set; }

		[JsonProperty("id")]
		public int? Id { get; set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("contentData")]
		public Dictionary<string, object> ContentData { get; set; }

		[JsonIgnore]
		public IPublishedElement Content { get; set; }


		public void ResetContents()
		{
			Id = null;
			Name = null;
			Alias = null;
			ContentData = null;
			Key = null;
			Content = null;
		}
	}


	public class AreaItem
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

		[JsonProperty("contentData")]
		public Dictionary<string, object> ContentData { get; set; }

		[JsonIgnore]
		public IPublishedElement Content { get; set; }
	}
}
