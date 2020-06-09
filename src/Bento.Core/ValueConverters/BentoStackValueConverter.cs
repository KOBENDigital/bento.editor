using System;
using System.Collections.Generic;
using System.Configuration;
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
	public class BentoStackValueConverter : IPropertyValueConverter
	{
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoStackValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
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

			var items = new List<StackItem>();
			try
			{
				items = JsonConvert.DeserializeObject<IEnumerable<StackItem>>(interString).ToList();
			}
			catch (Exception e)
			{
				var tempItems = JsonConvert.DeserializeObject<IEnumerable<OldStackItem>>(interString).ToList();

				foreach(var t in tempItems)
				{
					var item = new StackItem();
					item.Alias = ConfigurationManager.AppSettings["oldBentoDefaultLayout"];
					item.Areas = t.Areas;
					//old settings value, just rip it out!
					item.SettingsData = new Dictionary<string, object>();
					items.Add(item);
				}
			}

			var convertedItems = new List<StackItem>();

			//had to move to this because someone broke DI for propertyValueConverts between 8.2 and 8.5
			var embeddedContentService = Umbraco.Web.Composing.Current.Factory.GetInstance<IEmbeddedContentService>();

			foreach (var item in items)
			{
				if (item.SettingsData != null && item.SettingsData.Any())
				{
					item.Settings = embeddedContentService.ConvertValueToContent(item.SettingsData["key"].ToString(), (string)item.SettingsData["contentTypeAlias"], item.SettingsData);
				}

				foreach (var area in item.Areas.Where(x => x.Id != 0 || x.Key != Guid.Empty))
				{

					IPublishedElement content = null;
					if(area.Key != Guid.Empty && area.ContentData == null)
					{
						content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Key);
					} else if(area.Id != 0 && area.ContentData == null)
					{
						content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Id);
					} else if(area.ContentData != null)
					{
						// we need to convert the embedded item;
						content = embeddedContentService.ConvertValueToContent(area.Key.ToString(), (string)area.ContentData["contentTypeAlias"], area.ContentData);
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