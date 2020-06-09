using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	[DataEditor(
		Bento.Core.Constants.BentoItemDataEditor.EditorAlias,
		Bento.Core.Constants.BentoItemDataEditor.EditorName,
		"~/App_Plugins/Bento/bento.editor.html",
		ValueType = "JSON",
		Group = Bento.Core.Constants.BentoItemDataEditor.Group,
		Icon = "icon-umb-contour",
		HideLabel = false
	)]
	public class BentoItemDataEditor : DataEditor
	{
		public BentoItemDataEditor(ILogger logger) : base(logger)
		{

		}

		protected override IConfigurationEditor CreateConfigurationEditor() => new BentoItemConfigurationEditor();
		//protected override IDataValueEditor CreateValueEditor() => new DuploBlockDataValueEditor();
	}
}