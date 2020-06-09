using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoStackConfigurationEditor : ConfigurationEditor<BentoStackConfiguration>
	{
		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoStackConfiguration();
	}
}