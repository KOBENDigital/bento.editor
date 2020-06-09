using System.Reflection;
using Umbraco.Core.PropertyEditors;

namespace Bento.Core.DataEditors
{
	public class BentoStackConfiguration
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

		//[ConfigurationField(
		//	"layoutSettingsDoctypeAlias",
		//	"Layout Settings Doctype",
		//	"~/App_Plugins/Bento/Views/PreValueEditors/singlerequiredcontenttypepicker.html",
		//	Description = "Select the content type (this is used to configure the layout settings for each bento layout)."
		//)]
		//public string layoutSettingsDoctypeAlias { get; set; }

		//[ConfigurationField(
		//	"allowedDoctypeAliases",
		//	"Allowed content types",
		//	"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
		//	Description = "Optional. Select the content types the editor is allowed to use"
		//)]
		//public string AllowedDoctypeAliases { get; set; }

		//[ConfigurationField(
		//	"allowedElementAliases",
		//	"Allowed element types",
		//	"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
		//	Description = "Optional. Select the element types the editor is allowed to embed"
		//)]
		//public string AllowedElementAliases { get; set; }


		[ConfigurationField(
			"layouts",
			"Layouts",
			"/app_plugins/bento/prevalueeditors/bentolayout.editor.html",
			Description = "Set the available Layouts"
		)]
		public string Layouts { get; set; }

		[ConfigurationField(
			"useCssFile",
			"Use back office CSS",
			"boolean",
			Description = "[EXPERIMENTAL] allows you to use back office CSS to style your content.  Your CSS needs consider specifity to avoid clashes with the other back office styles."
		)]
		public bool UseCssFile { get; set; }

		[ConfigurationField(
			"cssFilePath",
			"CSS File Path",
			"textString",
			Description = "[EXPERIMENTAL] Path to your custom CSS file to provide styling for the back office"
		)]
		public string CssFilePath { get; set; }

		[ConfigurationField(
			"hideLabel",
			"Hide Label",
			"boolean",
			Description = "Select whether to hide the property label."
		)]
		public bool HideLabel { get; set; }
	}

}