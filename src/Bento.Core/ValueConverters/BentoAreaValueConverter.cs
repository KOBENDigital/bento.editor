using Bento.Core.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.Editors;
using System.Linq;


namespace Bento.Core.ValueConverters
{
	/// <summary>
	///     Converts JSON layout settings objects into <see cref="IPublishedElement" />.
	/// </summary>
	public sealed class BentoAreaValueConverter
	{
		private readonly IPublishedModelFactory _publishedModelFactory;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly Lazy<Dictionary<string, IContentType>> _elementTypes;
		private readonly PropertyEditorCollection _propertyEditors;
		private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
		private readonly IContentTypeService _contentTypeService;

		public BentoAreaValueConverter(
			IPublishedSnapshotAccessor publishedSnapshotAccessor,
			IPublishedModelFactory publishedModelFactory,
			PropertyEditorCollection propertyEditors,
			IContentTypeService contentTypeService,
			IDataTypeConfigurationCache dataTypeConfigurationCache
		)
		{
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_publishedModelFactory = publishedModelFactory;
			_propertyEditors = propertyEditors;
			_dataTypeConfigurationCache = dataTypeConfigurationCache;
			_elementTypes = new Lazy<Dictionary<string, IContentType>>(() => contentTypeService.GetAllElementTypes().ToDictionary(x => x.Alias));
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
			// if its of type ipublished content we dont need to do the content conversion only do this if its an elemnt type
			if (_elementTypes.Value.TryGetValue(data.ContentTypeAlias, out var elementType) == true)
			{

				Guid key = Guid.Empty;
				Guid.TryParse(data.Guid, out key);

				var contentData = JsonConvert.DeserializeObject(data.DataJson);
				var propValues = ((JObject)contentData).ToObject<Dictionary<string, object>>();

				//prepare all the property values.  Some properties need to have transformations (looking at you MultiTextStrings) before element construction
				foreach (var propertyType in elementType.CompositionPropertyTypes)
				{
					if (propValues.TryGetValue(propertyType.Alias, out var propVal) == true &&
							_propertyEditors.TryGet(propertyType.PropertyEditorAlias, out var propertyEditor) == true)
					{
						var configuration = _dataTypeConfigurationCache.GetConfiguration(propertyType.DataTypeKey);
						var contentPropertyData = new ContentPropertyData(propVal, configuration)
						{
							ContentKey = key,
							PropertyTypeKey = propertyType.Key,
							Files = Array.Empty<ContentPropertyFile>()
						};

						//key line here.  This is where the property value is converted from the editor to the final value
						var convertedValue = propertyEditor.GetValueEditor(configuration)?.FromEditor(contentPropertyData, propVal);

						propValues[propertyType.Alias] = convertedValue != null
								? convertedValue
								: null;
					}
				}

				//finally construct the element before returning
				IPublishedContentCache? publishedContentCache =
				_publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
				IPublishedContentType? publishedContentType = publishedContentCache?.GetContentType(data.ContentTypeAlias);
				IPublishedElement element = new PublishedElement(publishedContentType, key, propValues, true, PropertyCacheLevel.None, _publishedSnapshotAccessor);
				element = _publishedModelFactory.CreateModel(element);

				return element;

			}

			return null;

		}
	}
}