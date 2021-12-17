using System;
using System.Collections.Generic;
using System.Linq;
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
	public class BentoStackValueConverter : IPropertyValueConverter
	{
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly IEmbeddedContentService _embeddedContentService;

		public BentoStackValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, IEmbeddedContentService embeddedContentService)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_embeddedContentService = embeddedContentService;
		}

		public bool IsConverter(IPublishedPropertyType propertyType)
		{
			return BentoStackDataEditor.EditorAlias.Equals(propertyType.EditorAlias);
		}

		public bool? IsValue(object value, PropertyValueLevel level)
		{
			return !string.IsNullOrWhiteSpace(value?.ToString());
		}

		public Type GetPropertyValueType(IPublishedPropertyType propertyType)
		{
			return typeof(IEnumerable<StackItem>);
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

			IEnumerable<StackItem> items = null;
			try
			{
				items = JsonConvert.DeserializeObject<IEnumerable<StackItem>>(interString);
			}
			catch (Exception)
			{
				//ignored
			}

			if (items == null)
			{
				return null;
			}

			var convertedItems = new List<StackItem>();

			var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

			foreach (var item in items)
			{
				if (item.SettingsData != null && item.SettingsData.Any())
				{
					item.Settings = _embeddedContentService.ConvertValueToContent(item.SettingsData["key"].ToString(), (string)item.SettingsData["contentTypeAlias"], item.SettingsData);
				}

				foreach (var area in item.Areas.Where(x => x.Id != 0 || x.Key != Guid.Empty))
				{
					IPublishedElement content = null;
					if (area.Key != Guid.Empty && area.ContentData == null)
					{
						content = publishedContentCache.GetById(area.Id);
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

					if (content == null)
					{
						continue;
					}

					area.Key = content.Key;
					area.Content = content;
				}

				convertedItems.Add(item);
			}

			return convertedItems;
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