using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bento.Core.Constants;
using Bento.Core.DataEditors;
using Bento.Core.JsonConverters;
using Bento.Core.Models;
using Newtonsoft.Json;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration.Grid;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using BentoItemDataEditor = Bento.Core.Constants.BentoItemDataEditor;

namespace Bento.Core.Events
{
	public class ContentServiceEvents : IComponent
	{
		private static readonly IContentTypeBaseServiceProvider ContentTypeService =
			DependencyResolver.Current.GetService<IContentTypeBaseServiceProvider>();

		private static readonly IRelationService
			RelationService = DependencyResolver.Current.GetService<IRelationService>();

		private static readonly IDataTypeService
			DataTypeService = DependencyResolver.Current.GetService<IDataTypeService>();

		private static readonly IGridConfig GridConfig = DependencyResolver.Current.GetService<IGridConfig>();

		public void Initialize()
		{
			ContentService.Saved += ContentService_Saved;
			ContentService.Trashing += ContentService_Trashing;
		}

		public void Terminate() { }

		private static void ContentService_Trashing(IContentService sender, MoveEventArgs<IContent> moveEventArgs)
		{
			foreach (MoveEventInfo<IContent> trashingEntity in moveEventArgs.MoveInfoCollection)
			{
				List<IRelation> relations = RelationService.GetByChildId(trashingEntity.Entity.Id).ToList();

				if (relations.Any() == false)
				{
					continue;
				}

				IEnumerable<IRelationType> relationsTypes = RelationService
				                                            .GetAllRelationTypes(
					                                            relations.Select(x => x.RelationTypeId).ToArray())
				                                            .Where(x => x.Alias == RelationTypes.BentoItemsAlias);

				if (!relationsTypes.Any())
				{
					continue;
				}

				moveEventArgs.CancelOperation(new EventMessage("Bento setup",
				                                               $"This content is used in the following places: {string.Join(", ", relations.Select(x => sender.GetById(x.ParentId).Name))} (delete failed)",
				                                               EventMessageType.Error));
				break;
			}
		}

		private static void ContentService_Saved(IContentService contentService, ContentSavedEventArgs e)
		{
			foreach (IContent content in e.SavedEntities)
			{
				List<string> editors = new List<string>
				{
					Umbraco.Core.Constants.PropertyEditors.Aliases.Grid,
					BentoItemDataEditor.EditorAlias,
					BentoStackDataEditor.EditorAlias
				};

				foreach (Property contentProperty in content.Properties.Where(x => editors.Contains(x.PropertyType.PropertyEditorAlias)))
				{
					IDataType editor = DataTypeService.GetDataType(contentProperty.PropertyType.DataTypeId);

					IRelationType bentoBlocksRelationType = RelationService.GetRelationTypeByAlias(RelationTypes.BentoItemsAlias);

					if (bentoBlocksRelationType == null)
					{
						RelationType relationType = new RelationType(
							RelationTypes.BentoItemsName,
							RelationTypes.BentoItemsAlias,
							true,
							UmbracoObjectTypes.Document.GetGuid(),
							UmbracoObjectTypes.Document.GetGuid());

						RelationService.Save(relationType);

						bentoBlocksRelationType = RelationService.GetRelationTypeByAlias(RelationTypes.BentoItemsAlias);
					}

					//todo: this does work but it's a bit brute force...
					//i guess we could store the current relationships and then store the ones we're creating and compare them and then
					//delete the ones from the old list that arent in the new list? but that's a lot of db hits...
					IEnumerable<IRelation> rels = RelationService.GetByParentId(content.Id);
					foreach (IRelation currentRelation in rels.Where(x => x.RelationType.Id == bentoBlocksRelationType.Id))
					{
						RelationService.Delete(currentRelation);
					}

					if (contentProperty.PropertyType.PropertyEditorAlias == BentoItemDataEditor.EditorAlias)
					{
						foreach (Property.PropertyValue value in contentProperty.Values)
						{
							if (value.PublishedValue == null)
							{
								break;
							}

							var area = JsonConvert.DeserializeObject<Area>(value.PublishedValue.ToString());

							if (area.Id <= 0)
							{
								continue;
							}

							var bentoContent = contentService.GetById(area.Id);

							if (bentoContent == null)
							{
								continue;
							}

							BentoItemConfiguration config = (BentoItemConfiguration)editor.Configuration;

							ProcessRelationship(contentService, bentoContent, content, bentoBlocksRelationType, config.ItemDoctypeCompositionAlias);
						}
					}
					else
					{
						foreach (Property.PropertyValue value in contentProperty.Values)
						{
							if (value.PublishedValue == null)
							{
								break;
							}

							string valueString = value.PublishedValue?.ToString();

							if (string.IsNullOrWhiteSpace(valueString))
							{
								continue;
							}

							IEnumerable<StackItem> items = JsonConvert.DeserializeObject<IEnumerable<StackItem>>(valueString, new StackItemConverter());

							var itemList = items.Where(x => x.Areas != null && x.Areas.Any())
								.SelectMany(stackItem => stackItem.Areas.Where(x => x.Id > 0), (stackItem, x) => contentService.GetById(x.Id))
								.Where(bentoContent => bentoContent != null)
								.Distinct();

							BentoStackConfiguration config = (BentoStackConfiguration)editor.Configuration;

							foreach (IContent item in itemList)
							{
								ProcessRelationship(contentService, item, content, bentoBlocksRelationType, config.ItemDoctypeCompositionAlias);
							}
						}
					}
				}
			}
		}

		private static void ProcessRelationship(IContentService contentService, IContent block, IContent content,
		                                        IRelationType bentoBlocksRelationType, string blockDoctypeCompositionAlias)
		{
			//check that the control is a block i.e. it's composed of the block type composition doctype
			//IContent block = contentService.GetById(controlValueId);
			if (block == null)
			{
				return;
			}

			IContentTypeComposition contentTypeOf = ContentTypeService.GetContentTypeOf(block);
			if (contentTypeOf.ContentTypeComposition.FirstOrDefault(x => x.Alias == blockDoctypeCompositionAlias) == null)
			{
				return;
			}

			bool areRelated = RelationService.AreRelated(content, block, RelationTypes.BentoItemsAlias);

			if (areRelated)
			{
				return;
			}

			Relation relation = new Relation(content.Id, block.Id, bentoBlocksRelationType);

			//possibly record additional info on the comment field e.g. who created the relationship (so the editor of the parent)
			//var user = UserService.GetByUsername(HttpContext.Current.User.Identity.Name);
			//relation.Comment = user.Name;
			RelationService.Save(relation);
		}
	}
}
