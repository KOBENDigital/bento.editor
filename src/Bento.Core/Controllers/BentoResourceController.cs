using System;
using System.Collections.Generic;
using System.Linq;
using Bento.Core.Models;
using Bento.Core.Services.Interfaces;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Bento.Core.Controllers
{
	public class BentoResourceController : UmbracoAuthorizedJsonController
	{
		private readonly IContentService _contentService;
		private readonly IContentTypeService _contentTypeService;
		private readonly IPagingHelper _pagingHelper;
		private readonly IPluralizationServiceWrapper _pluralizationServiceWrapper;
		private readonly IRelationService _relationService;
		private readonly IUmbracoMapper _mapper;
		private readonly IUserService _userService;
		private readonly IVariationContextAccessor _variationContextAccessor;

		public BentoResourceController(IContentService contentService, IContentTypeService contentTypeService, IPagingHelper pagingHelper, IPluralizationServiceWrapper pluralizationServiceWrapper, IRelationService relationService, IUmbracoMapper mapper, IUserService userService, IVariationContextAccessor variationContextAccessor)
		{
			_contentService = contentService;
			_contentTypeService = contentTypeService;
			_pagingHelper = pagingHelper;
			_pluralizationServiceWrapper = pluralizationServiceWrapper;
			_relationService = relationService;
			_mapper = mapper;
			_userService = userService;
			_variationContextAccessor = variationContextAccessor;
		}

		public int GetLibraryFolderId(int contentId, string libraryFolderDoctypeAlias)
		{
			IContent libraryFolder = null;

			if (contentId >= 0)
			{
				var root = _contentService.GetAncestors(contentId).SingleOrDefault(x => x.Level == 1) ?? _contentService.GetById(contentId);

				var pageIndex = 0;
				const int pageSize = 100;
				var children = _pagingHelper.GetPagedChildren(_contentService, root.Id, pageIndex, pageSize, out var libraryTotalRecords);
				var hasNextPage = _pagingHelper.HasNextPage(pageIndex, pageSize, libraryTotalRecords);

				while (hasNextPage)
				{
					libraryFolder = children.SingleOrDefault(x => x.ContentType.Alias == libraryFolderDoctypeAlias);
					if (libraryFolder != null)
					{
						break;
					}

					pageIndex++;
					children = _pagingHelper.GetPagedChildren(_contentService, root.Id, pageIndex, pageSize, out libraryTotalRecords);
					hasNextPage = _pagingHelper.HasNextPage(pageIndex, pageSize, libraryTotalRecords);
				}
			}

			if (libraryFolder == null)
			{
				libraryFolder = _contentService.GetRootContent().SingleOrDefault(x => x.ContentType.Alias == libraryFolderDoctypeAlias);
			}

			if (libraryFolder == null)
			{
				throw new ArgumentException($"A library folder with the doctype alias of {libraryFolderDoctypeAlias} was not found (save failed)", libraryFolderDoctypeAlias);
			}

			return libraryFolder.Id;
		}

		public int GetItemTypeFolderId(int libraryFolderId, string itemTypeFolderDoctypeAlias, string selectedTypeName)
		{
			var libraryFolder = _contentService.GetById(libraryFolderId);

			IContent itemTypeFolder = null;
			string itemTypeFolderDoctypeName;

			if (selectedTypeName.Contains(' '))
			{
				var words = selectedTypeName.Split(' ');

				var pluralisedLastWord = _pluralizationServiceWrapper.Pluralize(_variationContextAccessor.VariationContext.Culture, words.Last());

				itemTypeFolderDoctypeName = $"{string.Join(" ", words.Take(words.Length - 1))} {pluralisedLastWord}";
			}
			else
			{
				itemTypeFolderDoctypeName = _pluralizationServiceWrapper.Pluralize(_variationContextAccessor.VariationContext.Culture, selectedTypeName);
			}

			var pageIndex = 0;
			const int pageSize = 100;
			var children = _pagingHelper.GetPagedChildren(_contentService, libraryFolder.Id, pageIndex, pageSize, out var totalRecords);
			var hasNextPage = _pagingHelper.HasNextPage(pageIndex, pageSize, totalRecords);

			while (hasNextPage)
			{
				itemTypeFolder = children.SingleOrDefault(x => x.Name == itemTypeFolderDoctypeName);
				if (itemTypeFolder != null)
				{
					break;
				}

				pageIndex++;
				children = _pagingHelper.GetPagedChildren(_contentService, libraryFolder.Id, pageIndex, pageSize, out totalRecords);
				hasNextPage = _pagingHelper.HasNextPage(pageIndex, pageSize, totalRecords);
			}

			if (itemTypeFolder == null)
			{
				itemTypeFolder = _contentService.Create(itemTypeFolderDoctypeName, libraryFolder.Key, itemTypeFolderDoctypeAlias);

				var publish = _contentService.SaveAndPublish(itemTypeFolder);
				if (publish.Result != PublishResultType.SuccessPublish)
				{
					throw new Exception($"Creating the 'Bento Block Type Folder' named '{itemTypeFolderDoctypeName}s' failed");
				}
			}

			return itemTypeFolder.Id;
		}

		public IEnumerable<ContentTypeBasic> GetAllowedContentTypes(string allowedDoctypeAliases = "")
		{
			if (string.IsNullOrWhiteSpace(allowedDoctypeAliases))
			{
				return Enumerable.Empty<ContentTypeBasic>();
			}

			var allowedTypesArray = allowedDoctypeAliases.Split(',');

			var types = _contentTypeService.GetAll().Where(x => allowedTypesArray.Contains(x.Alias)).ToList();

			if (types.Any() == false)
			{
				return Enumerable.Empty<ContentTypeBasic>();
			}

			//todo: can we map this to a much simpler object? i think we only need the id and the name?
			var basics = types.Where(type => type.IsElement == false).Select(_mapper.Map<IContentType, ContentTypeBasic>).OrderBy(x => x.Alias).ToList();

			return basics;
		}

		public IEnumerable<ContentTypeBasic> GetAllowedElementTypes(string allowedElementAliases = "")
		{
			if (string.IsNullOrWhiteSpace(allowedElementAliases))
			{
				return Enumerable.Empty<ContentTypeBasic>();
			}

			var allowedTypesArray = allowedElementAliases.Split(',');

			var types = _contentTypeService.GetAll().Where(x => allowedTypesArray.Contains(x.Alias)).ToList();

			if (types.Any() == false)
			{
				return Enumerable.Empty<ContentTypeBasic>();
			}

			//todo: can we map this to a much simpler object? i think we only need the id and the name?
			var basics = types.Where(type => type.IsElement).Select(_mapper.Map<IContentType, ContentTypeBasic>).OrderBy(x => x.Alias).ToList();

			return basics;
		}

		public Dictionary<string, List<RelationItem>> GetRelationsByChildId(int childId)
		{
			var relations = _relationService.GetByChildId(childId).ToArray();

			if (relations.Any() == false)
			{
				return null;
			}

			var relationsTypes = _relationService.GetAllRelationTypes(relations.Select(x => x.RelationTypeId).ToArray()).OrderBy(x => x.Name);

			var result = new Dictionary<string, List<RelationItem>>();

			foreach (var relationType in relationsTypes)
			{
				var items = new List<RelationItem>();

				foreach (var relation in relations)
				{
					var parent = _contentService.GetById(relation.ParentId);
					var creatorId = _userService.GetProfileById(parent.CreatorId);

					items.Add(new RelationItem
					{
						Id = parent.Id,
						Key = parent.Key,
						Icon = parent.ContentType.Icon,
						Name = parent.Name,
						Status = true,
						LastEdited = parent.UpdateDate,
						CreatedBy = creatorId.Name
					});
				}

				result.Add(relationType.Name, items);
			}

			return result;
		}
	}
}