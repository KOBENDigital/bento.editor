//todo: the files are being created in the wrong place! they end up in:
//Bento.Website\wwwroot\Views\Partials\Bento\RichTextBentoElement.cshtml
//but we need them in:
//Bento.Website\Views\Partials\Bento\RichTextBentoElement.cshtml
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bento.Core.Constants;
using Bento.Core.DataEditors;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using BentoItemDataEditor = Bento.Core.Constants.BentoItemDataEditor;
using File = System.IO.File;

namespace Bento.Core.NotificationHandlers
{
	public class ContentTypeServiceNotifications : INotificationHandler<ContentTypeSavedNotification>
	{
		//todo: not 100% sure why we're no just using standard di here?
		//private static readonly IDataTypeService DataTypeService = DependencyResolver.Current.GetService<IDataTypeService>();
		private readonly IDataTypeService DataTypeService;
		//private static readonly IIOHelper IOHelper = DependencyResolver.Current.GetService<IIOHelper>();
		private readonly IIOHelper IOHelper;

		public ContentTypeServiceNotifications(IDataTypeService dataTypeService, IIOHelper ioHelper)
		{
			DataTypeService = dataTypeService;
			IOHelper = ioHelper;
		}

		public void Handle(ContentTypeSavedNotification notification)
		{
			foreach (var content in notification.SavedEntities)
			{
				List<string> itemDoctypeCompositionAliases = new List<string>();

				IEnumerable<IDataType> bentoItemDataTypes = DataTypeService.GetByEditorAlias(BentoItemDataEditor.EditorAlias);
				itemDoctypeCompositionAliases.AddRange(bentoItemDataTypes
					.Select(dataType => (BentoItemConfiguration) dataType.Configuration)
					.Select(config => config.ItemDoctypeCompositionAlias));

				IEnumerable<IDataType> bentoStackDataTypes = DataTypeService.GetByEditorAlias(BentoStackDataEditor.EditorAlias);

				itemDoctypeCompositionAliases
					.AddRange(bentoStackDataTypes.Select(dataType => (BentoStackConfiguration) dataType.Configuration)
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
					backOfficeView.AppendLine($"\t\t<div class=\"title\">Backoffice Bento view for '{content.Name}' (alias: {content.Alias})</div>");
					backOfficeView.AppendLine($"\t\t<div class=\"sub-title\">To edit, please open the file at: ~/Views/Partials/Bento/{contentAlias}BackOffice.cshtml</div>");
					backOfficeView.AppendLine("\t</div>");
					backOfficeView.AppendLine("</div>");

					File.WriteAllText(IOHelper.MapPath($"~\\Views\\Partials\\Bento\\{contentAlias}BackOffice.cshtml"), backOfficeView.ToString());

					backofficeViewMessage = $"'~/Views/Partials/Bento/{contentAlias}BackOffice.cshtml'";
				}

				if (!string.IsNullOrWhiteSpace(websiteViewMessage) || !string.IsNullOrWhiteSpace(backofficeViewMessage))
				{
					notification.Messages.Add(
						new EventMessage(
							"Bento setup", $"Bento view(s) created ({websiteViewMessage}{(string.IsNullOrWhiteSpace(backofficeViewMessage) ? string.Empty : " and ")}{backofficeViewMessage}).",
							EventMessageType.Info)
						);
				}
			}
		}
	}
}