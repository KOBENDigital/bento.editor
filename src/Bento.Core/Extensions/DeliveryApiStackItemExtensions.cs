using Bento.Core.Models;
using System.Linq;
using Umbraco.Cms.Core.DeliveryApi;

namespace Bento.Core.Extensions

{
	internal static class DeliveryApiStackItemExtensions
	{
		internal static ApiStackItem CreateApiStackItem(this StackItem stackItem, IApiElementBuilder apiElementBuilder)
		{
			return new ApiStackItem
			{
				Alias = stackItem.Alias,
				Areas = stackItem.Areas.Select(x => x.CreateApiArea(apiElementBuilder)),
				Settings = apiElementBuilder.Build(stackItem.Settings)
			};
		}

		internal static ApiArea CreateApiArea(this Area area, IApiElementBuilder apiElementBuilder)
		{
			return new ApiArea
			{
				Key = area.Key,
				Id = area.Id,
				Alias = area.Alias,
				Name = area.Name,
				Content = apiElementBuilder.Build(area.Content)
			};
		}
	}
}
