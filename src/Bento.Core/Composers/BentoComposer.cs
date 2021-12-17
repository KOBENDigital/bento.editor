using Bento.Core.ContentApps;
using Bento.Core.Controllers;
using Bento.Core.NotificationHandlers;
using Bento.Core.Services;
using Bento.Core.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pluralize.NET;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Bento.Core.Composers
{
	public class BentoComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			//routing
			var globalSettings = new GlobalSettings();
			builder.Services.Configure<UmbracoPipelineOptions>(options =>
			{
				//options.AddFilter(new UmbracoPipelineFilter(nameof(BentoApiController))
				//{
				//	Endpoints = app => app.UseEndpoints(endpoints =>
				//	{
				//		endpoints.MapControllerRoute(
				//			"Bento Api Controller",
				//			globalSettings.UmbracoPath + "/backoffice/Api/Bento/{action}/{id}",
				//			new { Controller = "BentoApi", Action = "Index" });
				//	})
				//});
				options.AddFilter(new UmbracoPipelineFilter(nameof(BentoApiController))
				{
					Endpoints = app => app.UseEndpoints(endpoints =>
					{
						endpoints.MapControllerRoute(
							"Bento Backoffice Previews Controller",
							"/BentoApi/{action}/{id?}",
							new { Controller = "BentoApi", Action = "Index" });
					})
				});
			});

			//3rd party libraries
			builder.Services.AddUnique<Pluralize.NET.IPluralize, Pluralize.NET.Pluralizer>();

			//umbraco notifications
			builder.AddNotificationHandler<ContentMovingToRecycleBinNotification, ContentMovingToRecycleBinNotifications>();
			builder.AddNotificationHandler<ContentSavedNotification, ContentSavedNotifications>();
			builder.AddNotificationHandler<ContentTypeSavedNotification, ContentTypeServiceNotifications>();

			//services
			builder.Services.AddUnique<IPagingHelper, PagingHelper>();
			builder.Services.AddUnique<IPluralizationServiceWrapper, PluralizationServiceWrapper>();
			builder.Services.AddUnique<IEmbeddedContentService, EmbeddedContentService>();
			//builder.Services.AddUnique(f => Current.AppCaches.RuntimeCache);

			//todo: i can't work out why we're doing this?
			//controllers
			//builder.Register(f =>
			//{
			//	var s = f.GetInstance<IEmbeddedContentService>();
			//	var v = f.GetInstance<IVariationContextAccessor>();
			//	return new BentoApiController(s,v);
			//}, Lifetime.Request);

			//content apps
			builder.ContentApps().Append<RelationshipManager>();
		}
	}
}