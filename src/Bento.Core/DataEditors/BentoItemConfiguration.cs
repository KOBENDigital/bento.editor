using System.Reflection;
using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoItemConfiguration
	{
		[ConfigurationField(
"credits",
"Credits",
"/app_plugins/bento/prevalueeditors/bento.credits.html",
Description = "", HideLabel = true
)]
		public string Credits
		{
			get { return DataEditorConstants.Credits; }
			set { }
		}


		[ConfigurationField(
			"libraryFolderDoctypeAlias",
			"Library Folder content type",
			"~/App_Plugins/Bento/Views/PreValueEditors/singlerequiredcontenttypepicker.html",
			Description = "Select the library folder content type (this is where item folder types are created). This can be relative to the root node or the content root."
		)]
		public string LibraryFolderDoctypeAlias { get; set; }

		[ConfigurationField(
			"itemTypeFolderDoctypeAlias",
			"Item Type Folder content type",
			"~/App_Plugins/Bento/Views/PreValueEditors/singlerequiredcontenttypepicker.html",
			Description = "Select the item folder folder content type (this is where items are created)."
		)]
		public string ItemTypeFolderDoctypeAlias { get; set; }

		[ConfigurationField(
			"itemDoctypeCompositionAlias",
			"Item Doctype Composition content type",
			"~/App_Plugins/Bento/Views/PreValueEditors/singlerequiredcontenttypepicker.html",
			Description = "Select the item composition content type (this is used to identify the saved content as a Bento item)."
		)]
		public string ItemDoctypeCompositionAlias { get; set; }



		[ConfigurationField(
			"allowedDoctypeAliases",
			"Allowed content types",
			"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
			Description = "Optional. Select the content types the editor is allowed to use"
		)]
		public string AllowedDoctypeAliases { get; set; }

		[ConfigurationField(
			"allowedElementAliases",
			"Allowed element types",
			"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
			Description = "Optional. Select the element types the editor is allowed to embed"
		)]
		public string AllowedElementAliases { get; set; }

		[ConfigurationField(
			"hideLabel",
			"Hide Label",
			"boolean",
			Description = "Select whether to hide the property label."
		)]
		public bool HideLabel { get; set; }
	}
}