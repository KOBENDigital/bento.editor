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

				foreach (var area in item.Areas)
				{
					if (area.Contents == null)
						area.Contents = new List<AreaItem>();

					//Old "Single Item" Area setup
					//This is for when we have an OldStyleItem that has not been updated by backOffice
					IPublishedElement oldstyleContent = GetPublishedElementBy(embeddedContentService, area.Key, area.Id, area.ContentData);
					if (oldstyleContent != null)
					{
						area.Contents.Insert(0, new AreaItem()
						{
							Id = area.Id.Value,
							Name = area.Name,
							Alias = area.Alias,
							ContentData = area.ContentData,
							Key = oldstyleContent.Key,
							Content = oldstyleContent
						});
						area.ResetContents();
					}

					//New "Multiple Items" Area setup
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

					//This is to allow old "Single Item" Views to work using the first element of "Multiple Items"
					var oldStyleArea = area.Contents.FirstOrDefault();
					if (oldStyleArea != null)
					{
						area.Id = oldStyleArea.Id;
						area.Name = oldStyleArea.Name;
						area.Alias = oldStyleArea.Alias;
						area.ContentData = oldStyleArea.ContentData;
						area.Key = oldStyleArea.Key;
						area.Content = oldStyleArea.Content;
					}
				}

				convertedItems.Add(item);
			}

			return convertedItems;
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