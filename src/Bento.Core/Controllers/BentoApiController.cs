using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PublishedCache;
using Umbraco.Web;

namespace Bento.Core.Controllers
{
	public class BentoApiController : Umbraco.Web.Mvc.UmbracoAuthorizedController
	{
		private readonly IEmbeddedContentService _EmbeddedContentService;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		public BentoApiController(IEmbeddedContentService embeddedContentService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
		{
			_EmbeddedContentService = embeddedContentService ?? throw new ArgumentNullException(nameof(embeddedContentService));
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
		}

		public ActionResult LoadLibraryContent(int id, string culture)
		{
			if (!string.IsNullOrEmpty(culture))
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
				System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
			}

			var model = Umbraco.Content(id) ?? UmbracoContext.Content.GetById(true, id);

			return View($"~/Views/Partials/Bento/{model.ContentType.Alias}BackOffice.cshtml", model);
		}


		[System.Web.Http.HttpPost]
		public ActionResult LoadEmbeddedContent([FromBody]LoadEmbeddedContentRequest data)
		{
			if (data == null)
			{
				return null;
			}

			return ViewResult(data, $"~/Views/Partials/Bento/{data.ContentTypeAlias}BackOffice.cshtml");
			
		}


		/// <summary>
		/// used to load the styler for the back office
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[System.Web.Http.HttpPost]
		public ActionResult LoadEmbeddedStyler([FromBody]LoadEmbeddedContentRequest data)
		{
			if (data == null)
			{
				return null;
			}

			if (data == null)
			{
				return null;
			}

			return ViewResult(data, $"~/Views/Partials/Bento/Stylers/{data.ContentTypeAlias}.cshtml");
		}

		private ActionResult ViewResult(LoadEmbeddedContentRequest data, string viewPath)
		{
			if (!string.IsNullOrEmpty(data.Culture))
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(data.Culture);
				System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(data.Culture);
			}

			if (string.IsNullOrWhiteSpace(data.Guid))
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(data.ContentTypeAlias))
			{
				return null;
			}

			if (data.DataJson == null)
			{
				return null;
			}

			IPublishedElement content = _EmbeddedContentService.ConvertValueToContent(data.Guid, data.ContentTypeAlias, data.DataJson);

			return View(viewPath, content);

		}

		[System.Web.Http.HttpPost]
		public ActionResult LoadPreview([FromBody]LoadStackPreviewRequest data)
		{
			var items = new List<StackItem>();

			items = JsonConvert.DeserializeObject<IEnumerable<StackItem>>(data.StackItems).ToList();
			
			

			var convertedItems = new List<StackItem>();

			//had to move to this because someone broke DI for propertyValueConverts between 8.2 and 8.5


			foreach (var item in items)
			{
				if (item.SettingsData != null && item.SettingsData.Any())
				{
					item.Settings = _EmbeddedContentService.ConvertValueToContent(item.SettingsData["key"].ToString(), (string)item.SettingsData["contentTypeAlias"], item.SettingsData);
				}

				foreach (var area in item.Areas.Where(x => x.Id != 0 || x.Key != Guid.Empty))
				{

					IPublishedElement content = null;
					if (area.Key != Guid.Empty && area.ContentData == null)
					{
						content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Key);
					}
					else if (area.Id != 0 && area.ContentData == null)
					{
						content = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(area.Id);
					}
					else if (area.ContentData != null)
					{
						// we need to convert the embedded item;
						content = _EmbeddedContentService.ConvertValueToContent(area.Key.ToString(), (string)area.ContentData["contentTypeAlias"], area.ContentData);
					}

					if (content == null)
					{
						continue;
					}

					area.Key = content.Key;
					area.Content = content;
				}

				convertedItems.Add(item);
			}


			return View("~/views/_bentoPreview.cshtml", convertedItems);
		}


	}
}