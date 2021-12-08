using System;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Bento.Core.Controllers
{
	public class BentoApiController : UmbracoAuthorizedController
	{
		private readonly IEmbeddedContentService _EmbeddedContentService;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;

		public BentoApiController(IEmbeddedContentService embeddedContentService, IVariationContextAccessor variationContextAccessor, IUmbracoContextAccessor umbracoContextAccessor)
		{
			_variationContextAccessor = variationContextAccessor;
			_EmbeddedContentService = embeddedContentService ?? throw new ArgumentNullException(nameof(embeddedContentService));
			_umbracoContextAccessor = umbracoContextAccessor;
			
		}

		[HttpGet]
		public ActionResult LoadLibraryContent(int id, string culture)
		{
			if (!string.IsNullOrEmpty(culture))
			{
				_variationContextAccessor.VariationContext = new VariationContext(culture);
			}

			var ctx = _umbracoContextAccessor.UmbracoContext;
			var model = ctx.Content.GetById(true, id); //Umbraco.Content(id) ?? UmbracoContext.Content.GetById(true, id);

			return View($"~/Views/Partials/Bento/{model.ContentType.Alias}BackOffice.cshtml", model);
		}


		[HttpPost]
		public ActionResult LoadEmbeddedContent([FromBody]LoadEmbeddedContentRequest data)
		{
			if (data == null)
			{
				return null;
			}

			return BentoViewResult(data, $"~/Views/Partials/Bento/{data.ContentTypeAlias}BackOffice.cshtml");
			
		}


		/// <summary>
		/// used to load the styler for the back office
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
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

			return BentoViewResult(data, $"~/Views/Partials/Bento/Stylers/{data.ContentTypeAlias}.cshtml");
		}

		private ActionResult BentoViewResult(LoadEmbeddedContentRequest data, string viewPath)
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