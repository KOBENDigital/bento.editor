using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Bento.Core.DataEditors
{
	public class BentoStackDataValueEditor : DataValueEditor
	{
		public BentoStackDataValueEditor(ILocalizedTextService localizedTextService, IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer) : base(localizedTextService, shortStringHelper, jsonSerializer)
		{
		}

		public BentoStackDataValueEditor(ILocalizedTextService localizedTextService, IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper, DataEditorAttribute attribute) : base(localizedTextService, shortStringHelper, jsonSerializer, ioHelper, attribute)
		{
		}
	}
}