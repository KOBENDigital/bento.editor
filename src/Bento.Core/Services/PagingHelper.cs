using System;
using System.Collections.Generic;
using Bento.Core.Services.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bento.Core.Services
{
	public class PagingHelper : IPagingHelper
	{
		public IEnumerable<IContent> GetPagedChildren(IContentService sender, int id, long pageIndex, int pageSize,
		                                              out long totalRecords)
		{
			return sender.GetPagedChildren(id, pageIndex, pageSize, out totalRecords);
		}

		public bool HasNextPage(int pageIndex, int pageSize, long totalRecords)
		{
			return pageIndex < (int)Math.Ceiling(totalRecords / (decimal)pageSize);
		}
	}
}