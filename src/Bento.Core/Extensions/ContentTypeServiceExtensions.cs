using System;
using Umbraco.Cms.Core.Services;


namespace Bento.Core.Extensions
{
	internal static class ContentTypeServiceExtensions
	{
		public static string GetAliasByGuid(this IContentTypeService _, Guid id)
		{
			var cache = Current.AppCaches.RequestCache;

			return cache.GetCacheItem<string>(
				string.Concat("Bento.Core.Extensions.ContentTypeServiceExtensions.GetAliasById_", id),
				() =>
				{
					using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
						return scope.Database.ExecuteScalar<string>(
							"SELECT [cmsContentType].[alias] FROM [cmsContentType] INNER JOIN [umbracoNode] ON [cmsContentType].[nodeId] = [umbracoNode].[id] WHERE [umbracoNode].[uniqueID] = @0",
							id);
				});
		}
	}
}