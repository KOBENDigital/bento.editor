using Bento.Core.ContentApps;
using Bento.Core.Controllers;
using Bento.Core.Events;
using Bento.Core.Routing;
using Bento.Core.Services;
using Bento.Core.Services.Interfaces;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Bento.Core.Composers
{
	public class BentoComposer : IUserComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			//routing
			var globalSettings = new GlobalSettings();
			builder.Services.Configure<UmbracoPipelineOptions>(options =>
			{
				options.AddFilter(new UmbracoPipelineFilter(nameof(BentoApiController))
				{
					Endpoints = app => app.UseEndpoints(endpoints =>
					{
						endpoints.MapControllerRoute(
							"Bento Api Controller",
							globalSettings.UmbracoPath + "/backoffice/Api/Bento/{action}/{id}",
							new { Controller = "BentoApi", Action = "Index" });
					})
				});
			});

			//umbraco events
			builder.Components().Append<ContentServiceEvents>();
			builder.Components().Append<ContentTypeServiceEvents>();
			
			//services
			builder.Services.AddUnique<IPagingHelper, PagingHelper>();
			builder.Services.AddUnique<IPluralizationServiceWrapper, PluralizationServiceWrapper>();
			builder.Services.AddUnique<IEmbeddedContentService, EmbeddedContentService>();
			builder.Services.AddUnique(f => Current.AppCaches.RuntimeCache);

			//controllers
			builder.Register(f =>
			{
				var s = f.GetInstance<IEmbeddedContentService>();
				var v = f.GetInstance<IVariationContextAccessor>();
				return new BentoApiController(s,v);
			}, Lifetime.Request);

			//content apps
			builder.ContentApps().Append<RelationshipManager>();
		}
	}
}