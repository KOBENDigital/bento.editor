using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Extensions;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Bento.Core.Models;
using Bento.Core.Extensions;
using Umbraco.Cms.Core.PublishedCache;
using Newtonsoft.Json;

namespace Bento.Core.ValueConverters
{
	[DefaultPropertyValueConverter(typeof(JsonValueConverter))]
	public class BentoStackValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
	{

		private readonly IProfilingLogger _proflog;
		private readonly IApiElementBuilder _apiElementBuilder;
		private readonly BentoStackItemSettingsConverter _bentoStackSettingsItemConverter;
		private readonly BentoAreaValueConverter _bentoAreaConverter;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;



		public BentoStackValueConverter(IProfilingLogger proflog, IApiElementBuilder apiElementBuilder, BentoStackItemSettingsConverter bentoStackItemConverter, BentoAreaValueConverter bentoAreaConverter, IPublishedSnapshotAccessor publishedSnapshotAccessor)
		{
			_proflog = proflog;
			_apiElementBuilder = apiElementBuilder;
			_bentoStackSettingsItemConverter = bentoStackItemConverter;
			_bentoAreaConverter = bentoAreaConverter;
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
		}

		public override bool IsConverter(IPublishedPropertyType propertyType)
	=> propertyType.EditorAlias.InvariantEquals(Constants.BentoStackDataEditor.EditorAlias);

		public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
		{
			return typeof(IEnumerable<StackItem>);
		}

		public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
	=> PropertyCacheLevel.Element;

		public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType) => GetPropertyCacheLevel(propertyType);

		public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
		=> source?.ToString();

		public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
		{
			// NOTE: The intermediate object is just a JSON string, we don't actually convert from source -> intermediate since source is always just a JSON string
			using (!_proflog.IsEnabled(LogLevel.Debug) ? null : _proflog.DebugDuration<BentoStackValueConverter>(
					   $"ConvertPropertyToBentoStack ({propertyType.DataType.Id})"))
			{
				IEnumerable<StackItem> bentoStack = ConvertIntermediateToIEnumerableStackItem(owner, propertyType, referenceCacheLevel, inter, preview, true);
				if (bentoStack == null)
				{
					return null;
				}

				return bentoStack;
			}
		}

		public object ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview, bool expanding)
		{
			IEnumerable<StackItem> model = ConvertIntermediateToIEnumerableStackItem(owner, propertyType, referenceCacheLevel, inter, preview, expanding);

			return model.Any() ? model.Select(item => item.CreateApiStackItem(_apiElementBuilder))
					: Enumerable.Empty<ApiStackItem>();
		}

		public PropertyCacheLevel GetDeliveryApiPropertyCacheLevelForExpansion(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

		public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
				=> typeof(IEnumerable<ApiStackItem>);

		private IEnumerable<StackItem> ConvertIntermediateToIEnumerableStackItem(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview, bool expanding)
		{
			using (!_proflog.IsEnabled(LogLevel.Debug) ? null : _proflog.DebugDuration<BentoStackValueConverter>(
					   $"ConvertPropertyToBentoStack ({propertyType.DataType.Id})"))
			{
				// NOTE: this is to retain backwards compatability
				if (inter is null)
				{
					return Enumerable.Empty<StackItem>();
				}

				// NOTE: The intermediate object is just a JSON string, we don't actually convert from source -> intermediate since source is always just a JSON string
				if (inter is not string iEnumerableStackItemModelValue)
				{
					return null;
				}

				if (string.IsNullOrWhiteSpace(iEnumerableStackItemModelValue))
				{
					return null;
				}

				IEnumerable<StackItem> items = null;

				try
				{
					items = JsonConvert.DeserializeObject<IEnumerable<StackItem>>(iEnumerableStackItemModelValue);
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

				foreach (var item in items)
				{
					if (item.SettingsData != null && item.SettingsData.Any())
					{
						item.Settings = _bentoStackSettingsItemConverter.ConvertToElement((Dictionary<string, object>)item.SettingsData, referenceCacheLevel, preview);
					}

					foreach (var area in item.Areas.Where(x => x.Id != 0 || x.Key != Guid.Empty))
					{
						IPublishedElement content = null;
						if ((area.Key != Guid.Empty || area.Id != 0) && area.ContentData == null)
						{

							IPublishedContentCache? publishedContentCache =	_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

							if (area.Key != Guid.Empty)
							{
								content = publishedContentCache.GetById(area.Key);
							}
							else if (area.Id != 0)
							{
								content = publishedContentCache.GetById(area.Id);
							}
						}
						else if (area.ContentData != null)
						{
							// we need to convert the embedded item;
							content = _bentoAreaConverter.ConvertToElement(area, referenceCacheLevel, preview);
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
		}
	}
}
