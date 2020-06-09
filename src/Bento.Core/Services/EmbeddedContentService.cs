using System;
using System.Collections.Generic;
using System.Linq;
using Bento.Core.Extensions;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;

namespace Bento.Core.Services
{
	public class EmbeddedContentService : IEmbeddedContentService
	{
		private readonly IContentTypeService _contentTypeService;
		private readonly IDataTypeService _dataTypeService;
		private readonly IAppPolicyCache _runtimeCache;
		private readonly IPublishedModelFactory _publishedModelFactory;
		private readonly ILogger _logger;

		public EmbeddedContentService(IContentTypeService contentTypeService, IDataTypeService dataTypeService, IAppPolicyCache runtimeCache, IPublishedModelFactory publishedModelFactory, ILogger logger)
		{
			_contentTypeService = contentTypeService;
			_dataTypeService = dataTypeService;
			_runtimeCache = runtimeCache;
			_publishedModelFactory = publishedModelFactory;
			_logger = logger;
		}

		public IPublishedElement ConvertValueToContent(string guid, string contentTypeAlias, string dataJson)
		{
			var data = JsonConvert.DeserializeObject(dataJson);
			var propValues = ((JObject) data).ToObject<Dictionary<string, object>>();
			var content = ConvertValueToContent(guid, contentTypeAlias, propValues);

			return content;
		}

		public IPublishedElement ConvertValueToContent(string guid, string contentTypeAlias, IDictionary<string, object> propertyData)
		{
			var contentTypes = GetContentTypesByAlias(contentTypeAlias);
			var properties = new List<IPublishedProperty>();

			foreach (var jProp in propertyData)
			{
				var propType = contentTypes.PublishedContentType.GetPropertyType(jProp.Key);

				if (propType == null)
				{
					continue;
				}

				/* Because we never store the value in the database, we never run the property editors
				     * "ConvertEditorToDb" method however the property editors will expect their value to 
				     * be in a "DB" state so to get round this, we run the "ConvertEditorToDb" here before
				     * we go on to convert the value for the view. 
				     */
				var propEditor = _dataTypeService.GetByEditorAlias(propType.EditorAlias)?.FirstOrDefault();

				if (propEditor == null)
				{
					continue;
				}

				var propValueEditor = propEditor.Editor.GetValueEditor();

				var propPreValues = GetPreValuesCollectionByDataTypeId(propType.DataType.Id);

				var contentPropData = new ContentPropertyData(jProp.Value, propPreValues);

				var newValue = propValueEditor.FromEditor(contentPropData, jProp.Value);

				/* Now that we have the DB stored value, we actually need to then convert it into its
				 * XML serialized state as expected by the published property by calling ConvertDbToString
				 */
				var propType2 = contentTypes.ContentType.CompositionPropertyTypes.First(x => x.PropertyEditorAlias.InvariantEquals(propType.DataType.EditorAlias));

				Property prop2 = null;
				try
				{
					/* HACK: [LK:2016-04-01] When using the "Umbraco.Tags" property-editor, the converted DB value does
					     * not match the datatypes underlying db-column type. So it throws a "Type validation failed" exception.
					     * We feel that the Umbraco core isn't handling the Tags value correctly, as it should be the responsiblity
					     * of the "Umbraco.Tags" property-editor to handle the value conversion into the correct type.
					     * See: http://issues.umbraco.org/issue/U4-8279
					     */
					prop2 = new Property(propType2);
					prop2.SetValue(newValue);
				}
				catch (Exception ex)
				{
					_logger.Error(typeof(EmbeddedContentService), ex, "[Bento] Error creating Property object.");
				}

				if (prop2 == null)
				{
					continue;
				}

				var newValue2 = propValueEditor.ConvertDbToString(propType2, newValue, _dataTypeService);

				properties.Add(new DetachedPublishedProperty(propType, newValue2));
			}

			// Manually parse out the special properties
			propertyData.TryGetValue("name", out object nameObj);
			Guid.TryParse(guid, out Guid key);

			// Create the model based on our implementation of IPublishedElement
			IPublishedElement content = new DetachedPublishedElement(
				key,
				contentTypes.PublishedContentType,
				properties.ToArray());

			var publishedModelFactory = _publishedModelFactory;

			if (publishedModelFactory != null)
			{
				// Let the current model factory create a typed model to wrap our model
				content = publishedModelFactory.CreateModel(content);
			}

			return content;
		}

		private object GetPreValuesCollectionByDataTypeId(int dataTypeId)
		{
			return _runtimeCache.GetCacheItem(
				string.Concat("Bento.Core.Services.EmbeddedContentService.GetPreValuesCollectionByDataTypeId_", dataTypeId),
				() => _dataTypeService.GetDataType(dataTypeId).Configuration);
		}

		private ContentTypeContainer GetContentTypesByAlias(string contentTypeAlias)
		{
			if (Guid.TryParse(contentTypeAlias, out Guid contentTypeGuid))
			{
				contentTypeAlias = GetContentTypeAliasByGuid(contentTypeGuid);
			}

			return _runtimeCache.GetCacheItem(
				string.Concat("Bento.Core.Services.EmbeddedContentService.GetContentTypesByAlias_",
					contentTypeAlias),
				() => new ContentTypeContainer
				{
					PublishedContentType = new PublishedContentType(_contentTypeService.Get(contentTypeAlias), Current.PublishedContentTypeFactory),
					ContentType = _contentTypeService.Get(contentTypeAlias)
				});
		}

		private string GetContentTypeAliasByGuid(Guid contentTypeGuid)
		{
			return _runtimeCache.GetCacheItem(
				string.Concat("Bento.Core.Services.EmbeddedContentService.GetContentTypeAliasByGuid_", contentTypeGuid),
				() => _contentTypeService.GetAliasByGuid(contentTypeGuid));
		}
	}
}