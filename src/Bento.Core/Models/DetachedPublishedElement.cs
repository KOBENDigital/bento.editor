using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Bento.Core.Models {
	internal class DetachedPublishedElement : IPublishedElement
	{
		private readonly IPublishedContentType _contentType;
		private readonly IEnumerable<IPublishedProperty> _properties;
		private readonly bool _isPreviewing;
		private readonly Guid _key;

        
		public DetachedPublishedElement(
			Guid key,
			IPublishedContentType contentType,
			IEnumerable<IPublishedProperty> properties,
			bool isPreviewing = false)
		{
			_key = key;
			_contentType = contentType;
			_properties = properties;
			_isPreviewing = isPreviewing;
		}
		public IPublishedContentType ContentType => _contentType;
        
		public IPublishedProperty GetProperty(string alias) => _properties.FirstOrDefault(x => StringExtensions.InvariantEquals(x.PropertyType.Alias, alias));

		public Guid Key => _key;

		public IEnumerable<IPublishedProperty> Properties => _properties;

		public bool IsDraft => _isPreviewing;
	}
}