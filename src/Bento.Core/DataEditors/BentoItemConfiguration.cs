﻿using System.Reflection;
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
			"~/App_Plugins/Bento/Views/PreValueEditors/singlecontenttypepicker.html",
			Description = "Select the library folder content type (this is where item folder types are created). This can be relative to the root node or the content root. Only required if you are creating reusable items."
		)]
		public string LibraryFolderDoctypeAlias { get; set; }

		[ConfigurationField(
			"itemTypeFolderDoctypeAlias",
			"Item Type Folder content type",
			"~/App_Plugins/Bento/Views/PreValueEditors/singlecontenttypepicker.html",
			Description = "Select the item folder folder content type (this is where items are created). Only required if you are creating reusable items."
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
			"Allowed reusable library item types",
			"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
			Description = "Optional. Select the content types the editor is allowed to use. This requires that your library folders are configured"
		)]
		public string AllowedDoctypeAliases { get; set; }

		[ConfigurationField(
			"allowedElementAliases",
			"Allowed single use item types",
			"~/App_Plugins/Bento/Views/PreValueEditors/contenttypepicker.html",
			Description = "Optional. Select the single use element types the editor is allowed to create"
		)]
		public string AllowedElementAliases { get; set; }

		[ConfigurationField(
			"useCssFile",
			"Use back office CSS",
			"boolean",
			Description = "Allows you to use back office CSS to style your content."
		)]
		public bool UseCssFile { get; set; }

		[ConfigurationField(
			"cssFilePath",
			"CSS File",
			"/App_Plugins/Bento/Views/PreValueEditors/singlecssfilepicker.html",
			Description = "Path to your custom CSS file to provide styling for the back office."
		)]
		public string CssFilePath { get; set; }

		[ConfigurationField(
			"fontCssUrls",
			"Font CSS Urls",
			"textString",
			Description = "If you are using a custom web font from goolge or equivilant and would like to use it in the back office previews, paste the urls here separated by *'s"
		)]
		public string FontCssUrls { get; set; }


		[ConfigurationField(
	"useBlockSettingsCss",
	"Use block settings CSS",
	"boolean",
	Description = "Allows for some previewing of styles of the layout css.  Requires a view with the name the same and the alias of the document type used for your bento layout settings. Place this file in the /views/partials/bento/stylers folder"
)]
		public bool UseBlockSettingsCss { get; set; }

		[ConfigurationField(
"usePreviewJs",
"Use custom back office JS in preivews",
"boolean",
Description = "An advanced feature that allows you to run javascript code in your custom previews"
)]
		public bool UsePreviewJs { get; set; }

		[ConfigurationField(
"jsFilePath",
"JS file",
"/App_Plugins/Bento/Views/PreValueEditors/singlejsfilepicker.html",
Description = "enter the location of a custom JS  file that contains your common back office preview JS code"
)]
		public string JsFilePath { get; set; }

		[ConfigurationField(
"jsUserCode",
"User JS Code",
"/App_Plugins/Bento/Views/PreValueEditors/jscodeeditor.html",
Description = "Enter any JS that you wish to run when a preview loads.  E.G. backOfficeCropSwap(preview); The 'preview' object will give your custom JS the container shadowRoot of the preview output."
)]
		public string JsUserCode { get; set; }

		[ConfigurationField(
			"hideLabel",
			"Hide Label",
			"boolean",
			Description = "Select whether to hide the property label."
		)]
		public bool HideLabel { get; set; }
	}
}