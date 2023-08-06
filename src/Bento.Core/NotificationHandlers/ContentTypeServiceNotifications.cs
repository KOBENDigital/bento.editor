using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bento.Core.Constants;
using Bento.Core.DataEditors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using BentoItemDataEditor = Bento.Core.Constants.BentoItemDataEditor;
using File = System.IO.File;

namespace Bento.Core.NotificationHandlers
{
	public class ContentTypeServiceNotifications : INotificationHandler<ContentTypeSavedNotification>
	{
		private readonly IDataTypeService _dataTypeService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<ContentTypeServiceNotifications> _logger;

		public ContentTypeServiceNotifications(IDataTypeService dataTypeService, IWebHostEnvironment webHostEnvironment, ILogger<ContentTypeServiceNotifications> logger)
		{
			_dataTypeService = dataTypeService;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}

		public void Handle(ContentTypeSavedNotification notification)
		{
			try
			{
				var itemDoctypeCompositionAliases = new List<string>();

				//handle bento item (singular)
				var bentoItemDataTypes = _dataTypeService.GetByEditorAlias(BentoItemDataEditor.EditorAlias);
				itemDoctypeCompositionAliases.AddRange(bentoItemDataTypes
					.Select(dataType => (BentoItemConfiguration)dataType.Configuration)
					.Select(config => config.ItemDoctypeCompositionAlias));

				var bentoItemUseFramework = bentoItemDataTypes
					.Select(dataType => (BentoItemConfiguration)dataType.Configuration)
					.Select(config => config.UseFrontendFramework).FirstOrDefault();

				//handle bento stack
				var bentoStackDataTypes = _dataTypeService.GetByEditorAlias(BentoStackDataEditor.EditorAlias);
				itemDoctypeCompositionAliases.AddRange(bentoStackDataTypes.Select(dataType => (BentoStackConfiguration)dataType.Configuration)
					.Select(config => config.ItemDoctypeCompositionAlias));

				var bentoStackUseFramework = bentoStackDataTypes
					.Select(dataType => (BentoStackConfiguration)dataType.Configuration)
					.Select(config => config.UseFrontendFramework).FirstOrDefault();

				if (bentoStackUseFramework && bentoItemUseFramework)
				{
					//no views required
					return;
				}

				foreach (var content in notification.SavedEntities)
				{
					try
					{
						var compositionAliases = content.ContentTypeComposition.Select(x => x.Alias);

						var result = compositionAliases.Where(x => itemDoctypeCompositionAliases.Any(y => y == x));

						if (!result.Any())
						{
							continue;
						}

						var websiteViewMessage = string.Empty;
						var backofficeViewMessage = string.Empty;

						var view = new StringBuilder();
						view.AppendLine("@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage<IPublishedElement>");

						var contentAlias = content.Alias.First().ToString().ToUpper() + content.Alias.Substring(1);

						EnsureBentoDirectoryExists();
						EnsureBentoLayoutsDirectoryExists();

						if (!File.Exists(_webHostEnvironment.MapPathWebRoot($"~\\..\\Views\\Partials\\Bento\\{contentAlias}.cshtml")))
						{
							var websiteView = new StringBuilder();
							websiteView.Append(view);
							websiteView.AppendLine($"<p>View for Bento doctype '{content.Name}' (alias: {content.Alias})</p>");

							File.WriteAllText(_webHostEnvironment.MapPathWebRoot($"~\\..\\Views\\Partials\\Bento\\{contentAlias}.cshtml"), websiteView.ToString());

							websiteViewMessage = $"'~/Views/Bento/{contentAlias}.cshtml'";
						}

						if (!File.Exists(_webHostEnvironment.MapPathWebRoot($"~\\..\\Views\\Partials\\Bento\\{contentAlias}BackOffice.cshtml")))
						{
							var backOfficeView = new StringBuilder();
							backOfficeView.Append(view);
							backOfficeView.AppendLine("<div class=\"card hero\">");
							backOfficeView.AppendLine("\t<div class=\"card-content\">");
							backOfficeView.AppendLine($"\t\t<div class=\"title\">Backoffice Bento view for '{content.Name}' (alias: {content.Alias})</div>");
							backOfficeView.AppendLine($"\t\t<div class=\"sub-title\">To edit, please open the file at: ~/Views/Partials/Bento/{contentAlias}BackOffice.cshtml</div>");
							backOfficeView.AppendLine("\t</div>");
							backOfficeView.AppendLine("</div>");

							File.WriteAllText(_webHostEnvironment.MapPathWebRoot($"~\\..\\Views\\Partials\\Bento\\{contentAlias}BackOffice.cshtml"), backOfficeView.ToString());

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
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Error creating the Bento views");

						notification.Messages.Add(
							new EventMessage(
								"Bento setup", "Error creating the Bento views. Please see the log for more info.",
								EventMessageType.Warning)
						);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error getting Bento data types");

				notification.Messages.Add(
					new EventMessage(
						"Bento setup", "Error getting Bento data types. Please see the log for more info.",
						EventMessageType.Warning)
				);
			}
		}

		private void EnsureBentoLayoutsDirectoryExists()
		{
			if (!Directory.Exists(_webHostEnvironment.MapPathWebRoot("~\\..\\Views\\Partials\\Bento\\Layouts")))
			{
				Directory.CreateDirectory(_webHostEnvironment.MapPathWebRoot("~\\..\\Views\\Partials\\Bento\\Layouts"));
			}
		}

		private void EnsureBentoDirectoryExists()
		{
			if (!Directory.Exists(_webHostEnvironment.MapPathWebRoot("~\\..\\Views\\Partials\\Bento")))
			{
				Directory.CreateDirectory(_webHostEnvironment.MapPathWebRoot("~\\..\\Views\\Partials\\Bento"));
			}
		}
	}
}