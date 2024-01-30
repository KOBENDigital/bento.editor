using System;
using Bento.Core.Constants;
using Bento.Core.Models;
using Newtonsoft.Json;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;


namespace Bento.Core.ValueConverters
{
	[DefaultPropertyValueConverter(typeof(JsonValueConverter))]
	public class BentoItemValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
	{
		private readonly IProfilingLogger _proflog;
		private readonly IApiElementBuilder _apiElementBuilder;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly BentoAreaValueConverter _bentoAreaConverter;

		public BentoItemValueConverter(IProfilingLogger proflog, IPublishedSnapshotAccessor publishedSnapshotAccessor, BentoAreaValueConverter bentoAreaValueConverter, IApiElementBuilder apiElementBuilder)
		{
			_proflog = proflog;
			_apiElementBuilder = apiElementBuilder;
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_bentoAreaConverter = bentoAreaValueConverter;
		}



		public override bool IsConverter(IPublishedPropertyType propertyType)
=> propertyType.EditorAlias.InvariantEquals(BentoItemDataEditor.EditorAlias);


		public Type GetPropertyValueType(IPublishedPropertyType propertyType)
		{
			return typeof(IPublishedElement);
		}

		public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
			=> PropertyCacheLevel.Element;

		public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType) => GetPropertyCacheLevel(propertyType);

		public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
=> source?.ToString();

		public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
		{

			// NOTE: The intermediate object is just a JSON string, we don't actually convert from source -> intermediate since source is always just a JSON string
			using (!_proflog.IsEnabled(LogLevel.Debug) ? null : _proflog.DebugDuration<BentoItemValueConverter>(
					   $"ConvertPropertyToBentoItem ({propertyType.DataType.Id})"))
			{
				IPublishedElement bentoItem = ConvertIntermediateToBentoItem(owner, propertyType, referenceCacheLevel, inter, preview);
				if (bentoItem == null)
				{
					return null;
				}

				return bentoItem;
			}

		}

		public object ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview, bool expanding)
		{
			IPublishedElement model = ConvertIntermediateToBentoItem(owner, propertyType, referenceCacheLevel, inter, preview);

			return _apiElementBuilder.Build(model);
		}

		public PropertyCacheLevel GetDeliveryApiPropertyCacheLevelForExpansion(IPublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

		public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
				=> typeof(IPublishedElement);

		private IPublishedElement ConvertIntermediateToBentoItem(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
		{

			using (!_proflog.IsEnabled(LogLevel.Debug) ? null : _proflog.DebugDuration<BentoStackValueConverter>(
					   $"ConvertPropertyToBentoStack ({propertyType.DataType.Id})"))
			{
				// NOTE: this is to retain backwards compatability
				if (inter is null)
				{
					return null;
				}

				// NOTE: The intermediate object is just a JSON string, we don't actually convert from source -> intermediate since source is always just a JSON string
				if (inter is not string bentoItemModelValue)
				{
					return null;
				}

				if (string.IsNullOrWhiteSpace(bentoItemModelValue))
				{
					return null;
				}

				var area = JsonConvert.DeserializeObject<Area>(bentoItemModelValue);

				IPublishedElement content = null;
				if ((area.Key != Guid.Empty || area.Id != 0) && area.ContentData == null)
				{

					IPublishedContentCache? publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;

					if (area.Key != Guid.Empty)
					{
						content = publishedContentCache.GetById(preview, area.Key);
					}
					else if (area.Id != 0)
					{
						content = publishedContentCache.GetById(preview, area.Id);
					}
				}
				else if (area.ContentData != null)
				{
					// we need to convert the embedded item;
					content = _bentoAreaConverter.ConvertToElement(area, referenceCacheLevel, preview);
				}

				return content;

			}
		}
	}
}