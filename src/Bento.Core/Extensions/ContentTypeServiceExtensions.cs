////todo: i think the only reason this is set up like this is to extend the IContentTypeService with another extension method?
//using System;
//using Umbraco.Cms.Core.Cache;
//using Umbraco.Cms.Core.Scoping;
//using Umbraco.Cms.Core.Services;

//namespace Bento.Core.Extensions
//{
//	public static class ContentTypeServiceExtensions
//	{
//		public static string GetAliasByGuid(this IContentTypeService _, IAppCache appCache, IScopeProvider scopeProvider, Guid id)
//		{
//			return appCache.Get(string.Concat("Bento.Core.Extensions.ContentTypeServiceExtensions.GetAliasById_", id), () =>
//			{
//				using var scope = scopeProvider.CreateScope(autoComplete: true);
//				return scope.Database.ExecuteScalar<string>(
//					"SELECT [cmsContentType].[alias] FROM [cmsContentType] INNER JOIN [umbracoNode] ON [cmsContentType].[nodeId] = [umbracoNode].[id] WHERE [umbracoNode].[uniqueID] = @0",
//					id);
//			})?.ToString();
//		}
//	}
//}