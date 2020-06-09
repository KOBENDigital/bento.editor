using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	[DataEditor(
		Bento.Core.Constants.BentoStackDataEditor.EditorAlias,
		Bento.Core.Constants.BentoStackDataEditor.EditorName,
		"~/App_Plugins/Bento/bento.stack.editor.html",
		ValueType = "JSON",
		Group = Bento.Core.Constants.BentoItemDataEditor.Group,
		Icon = "icon-umb-contour",
		HideLabel = false
	)]
	public class BentStackPropertyEditor : DataEditor
	{
		public BentStackPropertyEditor(ILogger logger) : base(logger)
		{

		}

		protected override IConfigurationEditor CreateConfigurationEditor() => new BentoStackConfigurationEditor();
		//protected override IDataValueEditor CreateValueEditor() => new DuploBlockDataValueEditor();
	}
}