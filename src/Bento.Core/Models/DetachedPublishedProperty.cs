using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bento.Core.Models
{
	internal class DetachedPublishedProperty : IPublishedProperty
	{
		private readonly IPublishedPropertyType _propertyType;
		private readonly object _rawValue;
		private readonly Lazy<object> _objectValue;
		private readonly Lazy<object> _xpathValue;

		public DetachedPublishedProperty(IPublishedPropertyType propertyType, object value)
		: this(propertyType, value, false)
		{ }

		public DetachedPublishedProperty(IPublishedPropertyType propertyType, object value, bool isPreview)
		{
			_propertyType = propertyType;
			var isPreview1 = isPreview;

			_rawValue = value;

			var sourceValue = new Lazy<object>(() => _propertyType.ConvertSourceToInter(null, _rawValue, isPreview1));
			_objectValue = new Lazy<object>(() => _propertyType.ConvertInterToObject(null, PropertyCacheLevel.None, sourceValue.Value, isPreview1));
			_xpathValue = new Lazy<object>(() => _propertyType.ConvertInterToXPath(null, PropertyCacheLevel.None, sourceValue.Value, isPreview1));
		}

		public string PropertyTypeAlias => _propertyType.DataType.EditorAlias;

		public bool HasValue => DataValue != null && DataValue.ToString().Trim().Length > 0;

		public object DataValue => _rawValue;

		public object Value => _objectValue.Value;

		public object XPathValue => _xpathValue.Value;

		bool IPublishedProperty.HasValue(string culture, string segment)
		{
			return HasValue;
		}

		public object GetSourceValue(string culture = null, string segment = null)
		{
			return DataValue;
		}

		public object GetValue(string culture = null, string segment = null)
		{
			return Value;
		}

		public object GetXPathValue(string culture = null, string segment = null)
		{
			return XPathValue;
		}

		public IPublishedPropertyType PropertyType => _propertyType;

		public string Alias => _propertyType.Alias;
	}
}