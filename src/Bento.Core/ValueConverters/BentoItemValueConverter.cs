using System;
using Bento.Core.Constants;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace Bento.Core.ValueConverters
{
	public class BentoItemValueConverter : IPropertyValueConverter
	{
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly IEmbeddedContentService _embeddedContentService;

		public BentoItemValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, IEmbeddedContentService embeddedContentService)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_embeddedContentService = embeddedContentService;
		}

		public bool IsConverter(IPublishedPropertyType propertyType)
		{
			return BentoItemDataEditor.EditorAlias.Equals(propertyType.EditorAlias);
		}

		public bool? IsValue(object value, PropertyValueLevel level)
		{
			return !string.IsNullOrWhiteSpace(value?.ToString());
		}

		public Type GetPropertyValueType(IPublishedPropertyType propertyType)
		{
			return typeof(IPublishedElement);
		}

		public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
		{
			return PropertyCacheLevel.None;
		}

		public object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
		{
			var interString = source?.ToString();

			if (string.IsNullOrWhiteSpace(interString))
			{
				return null;
			}

			var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

			try
			{
				var area = JsonConvert.DeserializeObject<Area>(interString);

				IPublishedElement content = null;
				if (area.Key != Guid.Empty && area.ContentData == null)
				{
					content = publishedContentCache.GetById(area.Key);
				}
				else if (area.Id != 0 && area.ContentData == null)
				{
					content = publishedContentCache.GetById(area.Id);
				}
				else if (area.ContentData != null)
				{
					// we need to convert the embedded item;
					content = _embeddedContentService.ConvertValueToContent(area.Key.ToString(), (string)area.ContentData["contentTypeAlias"], area.ContentData);
				}

				return content;

			}
			catch (Exception e)
			{
				//todo: do we thing we can remove this now?
				// this is to catch legacy bento setups
				var content = publishedContentCache.GetById(Int32.Parse(interString));

				return content;
			}
		}

		public object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
		{
			return inter;
		}

		public object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
		{
			return null;
		}
	}
}