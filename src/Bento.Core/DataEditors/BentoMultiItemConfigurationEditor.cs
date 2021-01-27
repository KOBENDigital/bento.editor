using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoMultiItemConfigurationEditor : ConfigurationEditor<BentoMultiItemConfiguration>
	{
		public override object DefaultConfigurationObject
		{
			get;
		} = new BentoMultiItemConfiguration();
	}
}