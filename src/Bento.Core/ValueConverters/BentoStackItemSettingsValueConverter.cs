using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;


namespace Bento.Core.ValueConverters
{
	/// <summary>
	///     Converts JSON layout settings objects into <see cref="IPublishedElement" />.
	/// </summary>
	public sealed class BentoStackItemSettingsConverter
	{
		private readonly IPublishedModelFactory _publishedModelFactory;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoStackItemSettingsConverter(
			IPublishedSnapshotAccessor publishedSnapshotAccessor,
			IPublishedModelFactory publishedModelFactory)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_publishedModelFactory = publishedModelFactory;
		}

		public IPublishedElement? ConvertToElement(Dictionary<string, object>? data, PropertyCacheLevel referenceCacheLevel, bool preview)
		{
			IPublishedContentCache? publishedContentCache =
				_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

			// Only convert element types - content types will cause an exception when PublishedModelFactory creates the model
			IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType((string)data["contentTypeAlias"]);
			if (publishedContentType == null || publishedContentType.IsElement == false)
			{
				return null;
			}

			

			// Get the UDI from the deserialized object. If this is empty, we can fallback to checking the 'key' if there is one
			Guid key = Guid.Empty;
			if (data.TryGetValue("key", out var keyo))
			{
				Guid.TryParse(keyo!.ToString(), out key);
			}

			IPublishedElement element = new PublishedElement(publishedContentType, key, data, preview, referenceCacheLevel, _publishedSnapshotAccessor);
			element = _publishedModelFactory.CreateModel(element);

			return element;
		}


		//not sure we need this???
		public Type GetModelType(Guid contentTypeKey)
		{
			IPublishedContentCache? publishedContentCache =
				_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
			IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType(contentTypeKey);
			if (publishedContentType is not null && publishedContentType.IsElement)
			{
				return _publishedModelFactory.GetModelType(publishedContentType.Alias);
			}

			return typeof(IPublishedElement);
		}
	}

}