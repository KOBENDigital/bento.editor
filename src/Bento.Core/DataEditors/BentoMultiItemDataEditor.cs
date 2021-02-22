using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	[DataEditor(
		Bento.Core.Constants.BentoMultiItemDataEditor.EditorAlias,
		Bento.Core.Constants.BentoMultiItemDataEditor.EditorName,
		"~/App_Plugins/Bento/bento.editor.html",
		ValueType = "JSON",
		Group = Bento.Core.Constants.BentoItemDataEditor.Group,
		Icon = "icon-umb-contour",
		HideLabel = false
	)]
	public class BentoMultiItemDataEditor : DataEditor
	{
		public BentoMultiItemDataEditor(ILogger logger) : base(logger)
		{

		}

		protected override IConfigurationEditor CreateConfigurationEditor() => new BentoMultiItemConfigurationEditor();
	}
}