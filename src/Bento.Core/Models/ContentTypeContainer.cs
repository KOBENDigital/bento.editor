using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Bento.Core.Models {
	public class ContentTypeContainer
	{
		public PublishedContentType PublishedContentType { get; set; }

		public IContentType ContentType { get; set; }
	}
}