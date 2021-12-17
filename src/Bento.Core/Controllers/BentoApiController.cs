//todo: we need to figure out a way to lock this controller down?
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

namespace Bento.Core.Controllers
{
	public class BentoApiController : UmbracoPageController, IVirtualPageController
	{
		private readonly IEmbeddedContentService _embeddedContentService;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;
		private readonly IPublishedValueFallback _publishedValueFallback;

		public BentoApiController(ILogger<UmbracoPageController> logger, ICompositeViewEngine compositeViewEngine, IEmbeddedContentService embeddedContentService, IPublishedSnapshotAccessor publishedSnapshotAccessor, IVariationContextAccessor variationContextAccessor, IUmbracoContextAccessor umbracoContextAccessor, IPublishedValueFallback publishedValueFallback) : base(logger, compositeViewEngine)
		{
			_embeddedContentService = embeddedContentService;
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_variationContextAccessor = variationContextAccessor;
			_umbracoContextAccessor = umbracoContextAccessor;
			_publishedValueFallback = publishedValueFallback;
		}

		//[IsBackOffice]
		//[UmbracoUserTimeoutFilter]
		//[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
		//[DisableBrowserCache]
		//[UmbracoRequireHttps]
		[HttpGet]
		public IActionResult LoadLibraryContent(int id, string culture)
		{
			if (!string.IsNullOrEmpty(culture))
			{
				_variationContextAccessor.VariationContext = new VariationContext(culture);
			}

			var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
			var model = publishedContentCache.GetById(true, id);

			return View($"~/Views/Partials/Bento/{model.ContentType.Alias}BackOffice.cshtml", model);
		}

		[HttpPost]
		public IActionResult LoadEmbeddedContent([FromBody] LoadEmbeddedContentRequest data)
		{
			return BentoViewResult(data, $"~/Views/Partials/Bento/{data.ContentTypeAlias}BackOffice.cshtml");
		}

		/// <summary>
		/// used to load the styler for the back office
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult LoadEmbeddedStyler([FromBody] LoadEmbeddedContentRequest data)
		{
			return data == null ? null : BentoViewResult(data, $"~/Views/Partials/Bento/Stylers/{data.ContentTypeAlias}.cshtml");
		}

		public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
		{
			//todo: we just need to figure out how to make this return a valid bit of content, as long as it's not null this controller works
			var context = _umbracoContextAccessor.GetRequiredUmbracoContext();
			return context.Content.GetById(1067);
		}

		private IActionResult BentoViewResult(LoadEmbeddedContentRequest data, string viewPath)
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

			var content = _embeddedContentService.ConvertValueToContent(data.Guid, data.ContentTypeAlias, data.DataJson);

			return View(viewPath, content);
		}
	}
}