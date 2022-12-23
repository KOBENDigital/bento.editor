using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoItemConfigurationEditor : ConfigurationEditor<BentoItemConfiguration>
	{
		public BentoItemConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
		{
		}

		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoItemConfiguration();
	}
}