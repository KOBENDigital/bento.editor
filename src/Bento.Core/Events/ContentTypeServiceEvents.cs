using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bento.Core.Constants;
using Bento.Core.DataEditors;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using BentoItemDataEditor = Bento.Core.Constants.BentoItemDataEditor;
using File = System.IO.File;

namespace Bento.Core.Events
{
	public class ContentTypeServiceEvents : IComponent
	{
		private static readonly IDataTypeService
			DataTypeService = DependencyResolver.Current.GetService<IDataTypeService>();

		public void Initialize()
		{
			ContentTypeService.Saved += ContentService_Saved;
		}

		public void Terminate() { }

		private static void ContentService_Saved(IContentTypeService contentTypeService, SaveEventArgs<IContentType> e)
		{
			foreach (IContentType content in e.SavedEntities)
			{
				List<string> itemDoctypeCompositionAliases = new List<string>();

				IEnumerable<IDataType> bentoItemDataTypes = DataTypeService.GetByEditorAlias(BentoItemDataEditor.EditorAlias);
				itemDoctypeCompositionAliases.AddRange(bentoItemDataTypes
				                                       .Select(dataType => (BentoItemConfiguration)dataType.Configuration)
				                                       .Select(config => config.ItemDoctypeCompositionAlias));

				IEnumerable<IDataType> bentoStackDataTypes = DataTypeService.GetByEditorAlias(BentoStackDataEditor.EditorAlias);
				itemDoctypeCompositionAliases
					.AddRange(bentoStackDataTypes.Select(dataType => (BentoStackConfiguration)dataType.Configuration)
					                             .Select(config => config.ItemDoctypeCompositionAlias));

				IEnumerable<string> compositionAliases = content.ContentTypeComposition.Select(x => x.Alias);

				IEnumerable<string> result = compositionAliases.Where(x => itemDoctypeCompositionAliases.Any(y => y == x));

				if (!result.Any())
				{
					continue;
				}

				string websiteViewMessage = string.Empty;
				string backofficeViewMessage = string.Empty;

				StringBuilder view = new StringBuilder();
				view.AppendLine("@inherits Umbraco.Web.Mvc.UmbracoViewPage<IPublishedElement>");

				string contentAlias = content.Alias.First().ToString().ToUpper() + content.Alias.Substring(1);

				if (!Directory.Exists(IOHelper.MapPath("~\\Views\\Partials\\Bento")))
				{
					Directory.CreateDirectory(IOHelper.MapPath("~\\Views\\Partials\\Bento"));
					
				}

				if (!Directory.Exists(IOHelper.MapPath("~\\Views\\Partials\\Bento\\Layouts")))
				{
					Directory.CreateDirectory(IOHelper.MapPath("~\\Views\\Partials\\Bento\\Layouts"));
				}


				if (!File.Exists(IOHelper.MapPath($"~\\Views\\Partials\\Bento\\{contentAlias}.cshtml")))
				{
					StringBuilder websiteView = new StringBuilder();
					websiteView.Append(view);
					websiteView.AppendLine($"<p>View for Bento doctype '{content.Name}' (alias: {content.Alias})</p>");

					File.WriteAllText(IOHelper.MapPath($"~\\Views\\Partials\\Bento\\{contentAlias}.cshtml"), websiteView.ToString());

					websiteViewMessage = $"'~/Views/Bento/{contentAlias}.cshtml'";
				}

				if (!File.Exists(IOHelper.MapPath($"~\\Views\\Partials\\Bento\\{contentAlias}BackOffice.cshtml")))
				{
					StringBuilder backOfficeView = new StringBuilder();
					backOfficeView.Append(view);
					backOfficeView.AppendLine("<div class=\"card hero\">");
					backOfficeView.AppendLine("\t<div class=\"card-content\">");
					backOfficeView.AppendLine(
						$"\t\t<div class=\"title\">Backoffice Bento view for '{content.Name}' (alias: {content.Alias})</div>");
					backOfficeView.AppendLine(
						$"\t\t<div class=\"sub-title\">To edit, please open the file at: ~/Views/Partials/Bento/{contentAlias}BackOffice.cshtml</div>");
					backOfficeView.AppendLine("\t</div>");
					backOfficeView.AppendLine("</div>");

					File.WriteAllText(IOHelper.MapPath($"~\\Views\\Partials\\Bento\\{contentAlias}BackOffice.cshtml"),
					                  backOfficeView.ToString());

					backofficeViewMessage = $"'~/Views/Partials/Bento/{contentAlias}BackOffice.cshtml'";
				}

				if (!string.IsNullOrWhiteSpace(websiteViewMessage) || !string.IsNullOrWhiteSpace(backofficeViewMessage))
				{
					//todo: spent an hour trying to get this to work...
					//from what i can gather online, this has never been implemented...
					//bit of a shame as the user has no idea that the views have been created?!
					e.Messages.Add(new EventMessage("Bento setup",
					                                $"Bento view(s) created ({websiteViewMessage}{(string.IsNullOrWhiteSpace(backofficeViewMessage) ? string.Empty : " and ")}{backofficeViewMessage}).",
					                                EventMessageType.Info));
				}
			}
		}
	}
}