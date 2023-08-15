using System;
using System.Linq;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Extensions;

namespace Bento.Core.Controllers
{
	[IsBackOffice]
	[UmbracoUserTimeoutFilter]
	[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
	[DisableBrowserCache]
	[UmbracoRequireHttps]
	[MiddlewareFilter(typeof(UnhandledExceptionLoggerFilter))]
	public class BentoApiController : UmbracoPageController, IVirtualPageController
	{
		private readonly IEmbeddedContentService _embeddedContentService;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		private readonly IVariationContextAccessor _variationContextAccessor;
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;

		public BentoApiController(ILogger<UmbracoPageController> logger, ICompositeViewEngine compositeViewEngine, IEmbeddedContentService embeddedContentService, IPublishedSnapshotAccessor publishedSnapshotAccessor, IVariationContextAccessor variationContextAccessor, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine)
		{
			_embeddedContentService = embeddedContentService;
			_publishedSnapshotAccessor = publishedSnapshotAccessor;
			_variationContextAccessor = variationContextAccessor;
			_umbracoContextAccessor = umbracoContextAccessor;
		}

		[HttpGet]
		public IActionResult LoadLibraryContent(int id, string culture)
		{
			if (!string.IsNullOrEmpty(culture))
			{
				_variationContextAccessor.VariationContext = new VariationContext(culture);
			}

			var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content;
			var model = publishedContentCache?.GetById(true, id) ?? throw new Exception($"Unable to load library content using Id: {id}");

			return View($"~/Views/Partials/Bento/{model.ContentType.Alias}BackOffice.cshtml", model);
		}

		[HttpPost]
		public IActionResult LoadEmbeddedContent([FromBody] LoadEmbeddedContentRequest data)
		{
			return BentoViewResult(data, $"~/Views/Partials/Bento/{data.ContentTypeAlias}BackOffice.cshtml");
		}

		[HttpPost]
		public IActionResult LoadEmbeddedStyler([FromBody] LoadEmbeddedContentRequest data)
		{
			return data == null ? null : BentoViewResult(data, $"~/Views/Partials/Bento/Stylers/{data.ContentTypeAlias}.cshtml");
		}

		public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
		{
			//todo: this isn't ideal... but the controller needs to find a piece of content otherwise it won't find a route
			var context = _umbracoContextAccessor.GetRequiredUmbracoContext();
			return context.Content?.GetAtRoot().FirstOrDefault();
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