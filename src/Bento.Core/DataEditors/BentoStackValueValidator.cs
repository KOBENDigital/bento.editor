using System;
using System.Collections.Generic;
using System.Linq;
using Bento.Core.Models;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Bento.Core.DataEditors
{
	internal sealed class BentoStackValueValidator : ComplexEditorValidator
	{
		private readonly Lazy<Dictionary<string, IContentType>> _elementTypes;

		public BentoStackValueValidator(
				Lazy<Dictionary<string, IContentType>> elementTypes,
				IPropertyValidationService propertyValidationService)
				: base(propertyValidationService)
		{
			_elementTypes = elementTypes;
		}

		protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value)
		{
			if (value is JArray array && array.Any() == true)
			{
				var layouts = array.ToObject<IEnumerable<StackItem>>();
				if (layouts?.Any() == true)
				{
					foreach (var layout in layouts)
					{
						if (layout != null && layout.Areas?.Any() == true)
						{
							foreach (var area in layout.Areas)
							{

								if (area != null && area.ContentData.TryGetValue("contentTypeAlias", out var contentTypeAlias) == true && contentTypeAlias is string)
								{
									if (_elementTypes.Value.TryGetValue((string)contentTypeAlias, out var elementType) == true)
									{
										var elementValidation = new ElementTypeValidationModel(elementType.Alias, area.Key);

										foreach (var propertyType in elementType.CompositionPropertyTypes)
										{
											if (area.ContentData.TryGetValue(propertyType.Alias, out var bpv) == true)
											{
												elementValidation.AddPropertyTypeValidation(new PropertyTypeValidationModel(propertyType, bpv));
											}
										}

										yield return elementValidation;
									}
								}

							}
						}
					}
				}
			}
		}
	}
}
