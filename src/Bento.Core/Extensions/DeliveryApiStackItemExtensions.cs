using Bento.Core.Models;
using System;
using System.Linq;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Bento.Core.Extensions

{
	internal static class DeliveryApiStackItemExtensions
	{
		internal static ApiStackItem CreateApiStackItem(this StackItem stackItem, IApiElementBuilder apiElementBuilder)
		{
			var item = new ApiStackItem();

			item.Alias = stackItem.Alias;
				item.Areas = stackItem.Areas.Select(x => x.CreateApiArea(apiElementBuilder));
			item.Settings = stackItem.Settings != null ? apiElementBuilder.Build(stackItem.Settings) : null;

			return item;
		}

		internal static ApiArea CreateApiArea(this Area area, IApiElementBuilder apiElementBuilder)
		{

			var apiArea = new ApiArea();

			apiArea.Alias = area.Alias;
			apiArea.Content = apiElementBuilder.Build(area.Content);
			apiArea.Id = area.Id;
			apiArea.Key = area.Key;
			apiArea.Name = area.Name;

			return apiArea;
		}
	}
}
