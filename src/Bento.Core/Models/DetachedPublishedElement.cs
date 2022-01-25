using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Bento.Core.Models
{
	internal class DetachedPublishedElement : IPublishedElement
	{
		private readonly IEnumerable<IPublishedProperty> _properties;

		public DetachedPublishedElement(
			Guid key,
			IPublishedContentType contentType,
			IEnumerable<IPublishedProperty> properties,
			bool isPreviewing = false)
		{
			Key = key;
			ContentType = contentType;
			_properties = properties;
			IsDraft = isPreviewing;
		}

		public IPublishedContentType ContentType { get; }

		public IPublishedProperty GetProperty(string alias) => _properties.FirstOrDefault(x => x.PropertyType.Alias.InvariantEquals(alias));

		public Guid Key { get; }

		public IEnumerable<IPublishedProperty> Properties => _properties;

		public bool IsDraft { get; }
	}
}