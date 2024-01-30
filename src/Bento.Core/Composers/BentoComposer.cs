using Bento.Core.ContentApps;
using Bento.Core.Controllers;
using Bento.Core.NotificationHandlers;
using Bento.Core.Services;
using Bento.Core.Services.Interfaces;
using Bento.Core.ValueConverters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Bento.Core.Composers
{
	public class BentoComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			//routing
			builder.Services.Configure<UmbracoPipelineOptions>(options =>
			{
				options.AddFilter(new UmbracoPipelineFilter(nameof(BentoApiController))
				{
					Endpoints = app => app.UseEndpoints(endpoints =>
					{
						var globalSettings = app.ApplicationServices.GetRequiredService<IOptions<GlobalSettings>>().Value;
						var hostingEnvironment = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
						var backofficeArea = Umbraco.Cms.Core.Constants.Web.Mvc.BackOfficePathSegment;

						endpoints.MapControllerRoute(
							"Bento Backoffice Previews Controller",
							$"/{globalSettings.GetUmbracoMvcArea(hostingEnvironment)}/{backofficeArea}/Bento/{{action}}/{{id?}}",
							new { Controller = "BentoApi" });
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

			builder.Services.AddSingleton<BentoItemValueConverter>();
			builder.Services.AddSingleton<BentoAreaValueConverter>();
			builder.Services.AddSingleton<BentoStackItemSettingsConverter>();

			//todo: i can't work out why we're doing this? it was from umbraco 8 and the thinking is it might have something to do with variants and multilingual setup?
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