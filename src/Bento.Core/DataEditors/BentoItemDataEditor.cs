using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Bento.Core.DataEditors
{
	public class BentoItemDataEditor : IDataEditor
	{
		internal const string DataEditorViewPath = "~/App_Plugins/Bento/bento.editor.html";

		private readonly IIOHelper _ioHelper;
		private readonly ILocalizedTextService _localizedTextService;
		private readonly IShortStringHelper _shortStringHelper;
		private readonly IJsonSerializer _jsonSerializer;

		public BentoItemDataEditor(
			ILocalizedTextService localizedTextService,
			IShortStringHelper shortStringHelper,
			IJsonSerializer jsonSerializer,
			IIOHelper ioHelper)
		{
			_localizedTextService = localizedTextService;
			_shortStringHelper = shortStringHelper;
			_jsonSerializer = jsonSerializer;
			_ioHelper = ioHelper;
		}

		public string Alias => Bento.Core.Constants.BentoItemDataEditor.EditorAlias;

		public EditorType Type => EditorType.PropertyValue;

		public string Name => Bento.Core.Constants.BentoItemDataEditor.EditorName;

		public string Icon => "icon-umb-contour";

		public string Group => Bento.Core.Constants.BentoItemDataEditor.Group;

		public bool IsDeprecated => false;

		public IDictionary<string, object> DefaultConfiguration => default;

		public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

		public IConfigurationEditor GetConfigurationEditor() => new BentoItemConfigurationEditor(_ioHelper);

		/// <summary>
		/// i think this gets called when you set up the datatype
		/// </summary>
		/// <returns></returns>
		public IDataValueEditor GetValueEditor()
		{
			var editor = new BentoItemDataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer);
			editor.View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath);
			editor.ValueType = "JSON";
			return editor;
		}

		/// <summary>
		/// i think this gets called when you edit the datatype
		/// </summary>
		/// <returns></returns>
		public IDataValueEditor GetValueEditor(object configuration)
		{
			var hideLabel = false;

			if (configuration is Dictionary<string, object> config && config.ContainsKey("hideLabel"))
			{
				hideLabel = config["hideLabel"].TryConvertTo<bool>().Result;
			}

			var editor = new BentoItemDataValueEditor(_localizedTextService, _shortStringHelper, _jsonSerializer);
			editor.View = _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath);
			editor.ValueType = "JSON";
			editor.HideLabel = hideLabel;
			return editor;
		}
	}
}