using Bento.Core.ContentApps;
using Bento.Core.Controllers;
using Bento.Core.Events;
using Bento.Core.Routing;
using Bento.Core.Services;
using Bento.Core.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Bento.Core.Composers
{
	public class BentoComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			//routing
			composition.Components().Append<CustomRoutes>();

			//umbraco events
			composition.Components().Append<ContentServiceEvents>();
			composition.Components().Append<ContentTypeServiceEvents>();
			
			//services
			composition.Register<IPagingHelper, PagingHelper>();
			composition.Register<IPluralizationServiceWrapper, PluralizationServiceWrapper>();
			composition.Register<IEmbeddedContentService, EmbeddedContentService>();
			composition.Register(f => Current.AppCaches.RuntimeCache);

			//controllers
			composition.Register(f =>
			{
				var s = f.GetInstance<IEmbeddedContentService>();
				return new BentoApiController(s);
			}, Lifetime.Request);

			//content apps
			composition.ContentApps().Append<RelationshipManager>();
		}
	}
}