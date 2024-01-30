using Bento.Core.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
	public sealed class BentoAreaValueConverter
	{
		private readonly IPublishedModelFactory _publishedModelFactory;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoAreaValueConverter(
			IPublishedSnapshotAccessor publishedSnapshotAccessor,
			IPublishedModelFactory publishedModelFactory)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_publishedModelFactory = publishedModelFactory;
		}

		public IPublishedElement? ConvertToElement(Area data, PropertyCacheLevel referenceCacheLevel, bool preview)
		{
			IPublishedContentCache? publishedContentCache =
				_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

			// Only convert element types - content types will cause an exception when PublishedModelFactory creates the model
			IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType((string)data.ContentData["contentTypeAlias"]);
			if (publishedContentType == null || publishedContentType.IsElement == false)
			{
				return null;
			}

			// Get the UDI from the deserialized object. If this is empty, we can fallback to checking the 'key' if there is one
			Guid key = data.Key is Guid ? data.Key : Guid.Empty;

			IPublishedElement element = new PublishedElement(publishedContentType, key, data.ContentData, preview, referenceCacheLevel, _publishedSnapshotAccessor);
			element = _publishedModelFactory.CreateModel(element);

			return element;
		}


		//used specifically for backoffice previews
		public IPublishedElement? ConvertToElement(LoadEmbeddedContentRequest data)
		{
			IPublishedContentCache? publishedContentCache =
				_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

			IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType(data.ContentTypeAlias);
			if (publishedContentType == null || publishedContentType.IsElement == false)
			{
				return null;
			}

			Guid key = Guid.Empty;
			Guid.TryParse(data.Guid, out key);

			var contentData = JsonConvert.DeserializeObject(data.DataJson);
			var propValues = ((JObject)contentData).ToObject<Dictionary<string, object>>();

			IPublishedElement element = new PublishedElement(publishedContentType, key, propValues, true, PropertyCacheLevel.None, _publishedSnapshotAccessor);
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