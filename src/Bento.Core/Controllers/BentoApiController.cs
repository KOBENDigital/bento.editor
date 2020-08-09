using System;
using System.Web.Http;
using System.Web.Mvc;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Umbraco.Core.Models.PublishedContent;

namespace Bento.Core.Controllers
{
	public class BentoApiController : Umbraco.Web.Mvc.UmbracoAuthorizedController
	{
		private readonly IEmbeddedContentService _EmbeddedContentService;
		private readonly IVariationContextAccessor _variationContextAccessor;

		public BentoApiController(IEmbeddedContentService embeddedContentService, IVariationContextAccessor variationContextAccessor)
		{
			_variationContextAccessor = variationContextAccessor;
			_EmbeddedContentService = embeddedContentService ?? throw new ArgumentNullException(nameof(embeddedContentService));
			
		}

		public ActionResult LoadLibraryContent(int id, string culture)
		{
			if (!string.IsNullOrEmpty(culture))
			{
				_variationContextAccessor.VariationContext = new VariationContext(culture);
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
				_variationContextAccessor.VariationContext = new VariationContext(data.Culture);
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
	}
}