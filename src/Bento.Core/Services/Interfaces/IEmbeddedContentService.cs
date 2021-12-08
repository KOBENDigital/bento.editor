using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace Bento.Core.Services.Interfaces 
{
	public interface IEmbeddedContentService 
	{
		IPublishedElement ConvertValueToContent(string guid, string contentTypeAlias, string dataJson);
		IPublishedElement ConvertValueToContent(string guid, string contentTypeAlias, IDictionary<string,object> propertyData);
	}
}