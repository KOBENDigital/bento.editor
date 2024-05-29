using System;
using System.Collections.Generic;
using System.Linq;
using Bento.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;


namespace Bento.Core.DataEditors
{
	public class BentoItemDataValueEditor : DataValueEditor, IDataValueReference
	{

		private readonly IDataTypeConfigurationCache _dataTypeConfigurationCache;
		private readonly Lazy<Dictionary<string, IContentType>> _elementTypes;
		private readonly PropertyEditorCollection _propertyEditors;

		public BentoItemDataValueEditor(
						IContentTypeService contentTypeService,
						PropertyEditorCollection propertyEditors,
						IDataTypeConfigurationCache dataTypeConfigurationCache,
						ILocalizedTextService localizedTextService,
						IShortStringHelper shortStringHelper,
						IJsonSerializer jsonSerializer,
						IPropertyValidationService propertyValidationService)
						: base(localizedTextService, shortStringHelper, jsonSerializer)
		{
			_dataTypeConfigurationCache = dataTypeConfigurationCache;
			_elementTypes = new Lazy<Dictionary<string, IContentType>>(() => contentTypeService.GetAllElementTypes().ToDictionary(x => x.Alias));
			_propertyEditors = propertyEditors;

			Validators.Add(new BentoItemValueValidator(_elementTypes, propertyValidationService));
		}

		public override object ToEditor(IProperty property, string? culture = null, string? segment = null)
		{
			var value = base.ToEditor(property, culture, segment)?.ToString();
			if (string.IsNullOrWhiteSpace(value) == false)
			{
				var area = JsonConvert.DeserializeObject<Area>(value);
				if (area != null && area.ContentData?.TryGetValue("contentTypeAlias", out var contentTypeAlias) == true && contentTypeAlias is string)
				{
					if (_elementTypes.Value.TryGetValue((string)contentTypeAlias, out var elementType) == true)
					{
						foreach (var propertyType in elementType.CompositionPropertyTypes)
						{
							if (area.ContentData.TryGetValue(propertyType.Alias, out var propVal) == true)
							{
								propertyType.Variations = ContentVariation.Nothing;

								var fakeProperty = new Property(propertyType);
								fakeProperty.SetValue(propVal);

								if (_propertyEditors.TryGet(propertyType.PropertyEditorAlias, out var propertyEditor) == true)
								{
									var convertedValue = propertyEditor.GetValueEditor()?.ToEditor(fakeProperty);

									area.ContentData[propertyType.Alias] = convertedValue is not null
											? JToken.FromObject(convertedValue)
											: null;
								}
								else
								{
									area.ContentData[propertyType.Alias] = fakeProperty.GetValue()?.ToString();
								}
							}
						}
					}
					return area;
				}
			}
			return null;
		}

		public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
		{
			var value = editorValue.Value?.ToString();
			if (string.IsNullOrWhiteSpace(value) == false)
			{
				var area = JsonConvert.DeserializeObject<Area>(value);
				if (area != null && area.ContentData?.TryGetValue("contentTypeAlias", out var contentTypeAlias) == true && contentTypeAlias is string)
				{
					if (_elementTypes.Value.TryGetValue((string)contentTypeAlias, out var elementType) == true)
					{
						foreach (var propertyType in elementType.CompositionPropertyTypes)
						{
							if (area.ContentData.TryGetValue(propertyType.Alias, out var propVal) == true &&
									_propertyEditors.TryGet(propertyType.PropertyEditorAlias, out var propertyEditor) == true)
							{
								var configuration = _dataTypeConfigurationCache.GetConfiguration(propertyType.DataTypeKey);
								var contentPropertyData = new ContentPropertyData(propVal, configuration)
								{
									ContentKey = area.Key,
									PropertyTypeKey = propertyType.Key,
									Files = Array.Empty<ContentPropertyFile>()
								};

								var convertedValue = propertyEditor.GetValueEditor(configuration)?.FromEditor(contentPropertyData, propVal);

								area.ContentData[propertyType.Alias] = convertedValue != null
										? JToken.FromObject(convertedValue)
										: null;
							}
						}
					}
				}
				return JsonConvert.SerializeObject(area, Formatting.None);
			}
			return base.FromEditor(editorValue, currentValue);
		}

		public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
		{
			if (value is string str && string.IsNullOrWhiteSpace(str) == false && str.DetectIsJson() == true)
			{
				var area = JsonConvert.DeserializeObject<Area>(str);

				if (area != null && area.ContentData?.TryGetValue("contentTypeAlias", out var contentTypeAlias) == true && contentTypeAlias is string)
				{

					if (_elementTypes.Value.TryGetValue((string)contentTypeAlias, out var elementType) == true)
					{
						foreach (var propertyType in elementType.CompositionPropertyTypes)
						{
							if (area.ContentData.TryGetValue(propertyType.Alias, out var bpv) == true &&
									_propertyEditors.TryGet(propertyType.PropertyEditorAlias, out var editor) == true &&
									editor?.GetValueEditor() is IDataValueReference dvr)
							{
								foreach (var reference in dvr.GetReferences(bpv))
								{
									yield return reference;
								}
							}
						}
					}
				}
			}
		}


	}
}