using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoItemConfigurationEditor : ConfigurationEditor<BentoItemConfiguration>
	{
		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoItemConfiguration();
	}
}