using System;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;

namespace Bento.Core.Routing
{
	public class CustomRoutes : IComponent
	{
		private readonly IGlobalSettings _globalSettings;

		public CustomRoutes(IGlobalSettings globalSettings)
		{
			_globalSettings = globalSettings;
		}

		public void Initialize()
		{
			RouteTable.Routes.MapRoute("BentoApiIndex", _globalSettings.GetUmbracoMvcArea() + "/backoffice/Api/Bento/{action}/{id}", new
			{
				controller = "BentoApi",
				action = "Index",
				id = UrlParameter.Optional
			});
		}

		public void Terminate() { }
	}
}