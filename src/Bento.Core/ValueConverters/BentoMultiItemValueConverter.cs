using System;
using System.Collections.Generic;
using System.Linq;
using Bento.Core.Constants;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PublishedCache;

namespace Bento.Core.ValueConverters
{
	public class BentoMultiItemValueConverter : IPropertyValueConverter
	{
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoMultiItemValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
		}

		public bool IsConverter(IPublishedPropertyType propertyType)
		{
			return BentoMultiItemDataEditor.EditorAlias.Equals(propertyType.EditorAlias);
		}

		public bool? IsValue(object value, PropertyValueLevel level)
		{
			return !string.IsNullOrWhiteSpace(value?.ToString());
		}

		public Type GetPropertyValueType(IPublishedPropertyType propertyType)
		{
			return typeof(Area);
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

			try
			{
				var area = JsonConvert.DeserializeObject<Area>(interString);
				//ToDo: should we make a BentoItem model (the same way we get StackItem from "Bento Stack") without the unwanted Area properties?

				//had to move to this because someone broke DI for propertyValueConverts between 8.2 and 8.5
				var embeddedContentService = Umbraco.Web.Composing.Current.Factory.GetInstance<IEmbeddedContentService>();

				if (area.Contents == null)
					area.Contents = new List<AreaItem>();

				foreach (var areaItem in area.Contents.Where(x => x.Id != 0 || x.Key != Guid.Empty))
				{
					IPublishedElement content = GetPublishedElementBy(embeddedContentService, areaItem.Key, areaItem.Id, areaItem.ContentData);

					if (content == null)
					{
						continue;
					}

					areaItem.Key = content.Key;
					areaItem.Content = content;
				}

				return area;
			}
			catch (Exception e)
			{
				// this is to catch legacy bento setups

				//No need as this never existed before
				return null;
			}
		}

		private IPublishedElement GetPublishedElementBy(IEmbeddedContentService embeddedContentService, Guid? key, int? id, Dictionary<string, object> contentData)
		{
			IPublishedElement content = null;
			if (key.HasValue && key != Guid.Empty && contentData == null)
			{
				content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(key.Value);
			}
			else if (id.HasValue && id != 0 && contentData == null)
			{
				content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(id.Value);
			}
			else if (contentData != null)
			{
				// we need to convert the embedded item;
				content = embeddedContentService.ConvertValueToContent(key.ToString(), (string)contentData["contentTypeAlias"], contentData);
			}

			return content;
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