using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Bento.Core.ContentApps
{
	public class RelationshipManager : IContentAppFactory
	{
		public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
		{
			switch (source)
			{
				case IMedia _:
					return null;
				case IContent _:
					var app = new ContentApp
					{
						Alias = "bentoRelationshipManager",
						Name = "Usage", //todo: how do we localise this? this pr seems to suggest you can https://github.com/umbraco/Umbraco-CMS/pull/4549/commits/730a2f7768185d718a2fc0223ae43e73d0b38ba8
						Icon = "icon-trafic",
						View = "/App_Plugins/BentoRelationshipManager/relationshipmanager.html",
						Weight = 101
					};

					return app;
			}
			return null;
			//throw new NotSupportedException($"Object type {source.GetType()} is not supported here.");
		}
	}
}