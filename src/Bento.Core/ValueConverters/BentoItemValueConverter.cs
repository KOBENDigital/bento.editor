using System;
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
	public class BentoItemValueConverter : IPropertyValueConverter
	{
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoItemValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
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

			try
			{
				var area = JsonConvert.DeserializeObject<Area>(interString);
				
				//had to move to this because someone broke DI for propertyValueConverts between 8.2 and 8.5
				var embeddedContentService = Umbraco.Web.Composing.Current.Factory.GetInstance<IEmbeddedContentService>();



				IPublishedElement content = null;
				if (area.Key != Guid.Empty && area.ContentData == null)
				{
					content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Key);
				}
				else if (area.Id != 0 && area.ContentData == null)
				{
					content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Id);
				}
				else if (area.ContentData != null)
				{
					// we need to convert the embedded item;
					content = embeddedContentService.ConvertValueToContent(area.Key.ToString(), (string)area.ContentData["contentTypeAlias"], area.ContentData);
				}

				return content;

			} catch(Exception e)
			{

				// this is to catch legacy bento setups
				var content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(Int32.Parse(interString));

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