using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoStackConfigurationEditor : ConfigurationEditor<BentoStackConfiguration>
	{
		public BentoStackConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
		{
		}

		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoStackConfiguration();
	}
}