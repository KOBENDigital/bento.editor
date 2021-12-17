using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Bento.Core.Services.Interfaces
{
	public interface IPagingHelper
	{
		IEnumerable<IContent> GetPagedChildren(IContentService sender, int id, long pageIndex, int pageSize, out long totalRecords);

		bool HasNextPage(int pageIndex, int pageSize, long totalRecords);
	}
}