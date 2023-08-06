using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Bento.Core.DataEditors
{
	public class BentoStackConfigurationEditor : ConfigurationEditor<BentoStackConfiguration>
	{
		public BentoStackConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
		{
		}

		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoStackConfiguration();
	}
}