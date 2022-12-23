using System.Collections.Generic;
using System.Linq;
using Bento.Core.Constants;
using Bento.Core.DataEditors;
using Bento.Core.JsonConverters;
using Bento.Core.Models;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using BentoItemDataEditor = Bento.Core.Constants.BentoItemDataEditor;

namespace Bento.Core.NotificationHandlers
{
	public class ContentSavedNotifications : INotificationHandler<ContentSavedNotification>
	{
		private readonly IContentTypeBaseServiceProvider _contentTypeService;
		private readonly IRelationService _relationService;
		private readonly IDataTypeService _dataTypeService;
		private readonly IContentService _contentService;

		public ContentSavedNotifications(IContentTypeBaseServiceProvider contentTypeService, IRelationService relationService, IDataTypeService dataTypeService, IContentService contentService)
		{
			_contentTypeService = contentTypeService;
			_relationService = relationService;
			_dataTypeService = dataTypeService;
			_contentService = contentService;
		}

		public void Handle(ContentSavedNotification notification)
		{
			foreach (var content in notification.SavedEntities)
			{
				List<string> editors = new List<string>
				{
					Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.Grid,
					BentoItemDataEditor.EditorAlias,
					BentoStackDataEditor.EditorAlias
				};

				foreach (IProperty contentProperty in content.Properties.Where(x => editors.Contains(x.PropertyType.PropertyEditorAlias)))
				{
					IDataType editor = _dataTypeService.GetDataType(contentProperty.PropertyType.DataTypeId);

					IRelationType bentoBlocksRelationType = _relationService.GetRelationTypeByAlias(RelationTypes.BentoItemsAlias);

					if (bentoBlocksRelationType == null)
					{
						RelationType relationType = new RelationType(
							RelationTypes.BentoItemsName,
							RelationTypes.BentoItemsAlias,
							true,
							UmbracoObjectTypes.Document.GetGuid(),
							UmbracoObjectTypes.Document.GetGuid());

						_relationService.Save(relationType);

						bentoBlocksRelationType = _relationService.GetRelationTypeByAlias(RelationTypes.BentoItemsAlias);
					}

					//todo: this does work but it's a bit brute force...
					//i guess we could store the current relationships and then store the ones we're creating and compare them and then
					//delete the ones from the old list that arent in the new list? but that's a lot of db hits...
					IEnumerable<IRelation> rels = _relationService.GetByParentId(content.Id);
					foreach (IRelation currentRelation in rels.Where(x => x.RelationType.Id == bentoBlocksRelationType.Id))
					{
						_relationService.Delete(currentRelation);
					}

					if (contentProperty.PropertyType.PropertyEditorAlias == BentoItemDataEditor.EditorAlias)
					{
						foreach (var propertyValue in contentProperty.Values)
						{
							var value = (Property.PropertyValue) propertyValue;
							if (value.PublishedValue == null)
							{
								break;
							}

							var area = JsonConvert.DeserializeObject<Area>(value.PublishedValue.ToString());

							if (area.Id <= 0)
							{
								continue;
							}

							var bentoContent = _contentService.GetById(area.Id);

							if (bentoContent == null)
							{
								continue;
							}

							BentoItemConfiguration config = (BentoItemConfiguration)editor.Configuration;

							ProcessRelationship(bentoContent, content, bentoBlocksRelationType, config.ItemDoctypeCompositionAlias);
						}
					}
					else
					{
						foreach (var propertyValue in contentProperty.Values)
						{
							var value = (Property.PropertyValue) propertyValue;
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
								.SelectMany(stackItem => stackItem.Areas.Where(x => x.Id > 0), (stackItem, x) => _contentService.GetById(x.Id))
								.Where(bentoContent => bentoContent != null)
								.Distinct();

							BentoStackConfiguration config = (BentoStackConfiguration)editor.Configuration;

							foreach (IContent item in itemList)
							{
								ProcessRelationship(item, content, bentoBlocksRelationType, config.ItemDoctypeCompositionAlias);
							}
						}
					}
				}
			}
		}

		private void ProcessRelationship(IContent block, IContent content, IRelationType bentoBlocksRelationType, string blockDoctypeCompositionAlias)
		{
			if (block == null)
			{
				return;
			}

			IContentTypeComposition contentTypeOf = _contentTypeService.GetContentTypeOf(block);
			if (contentTypeOf.ContentTypeComposition.FirstOrDefault(x => x.Alias == blockDoctypeCompositionAlias) == null)
			{
				return;
			}

			bool areRelated = _relationService.AreRelated(content, block, RelationTypes.BentoItemsAlias);

			if (areRelated)
			{
				return;
			}

			Relation relation = new Relation(content.Id, block.Id, bentoBlocksRelationType);

			//possibly record additional info on the comment field e.g. who created the relationship (so the editor of the parent)
			//var user = UserService.GetByUsername(HttpContext.Current.User.Identity.Name);
			//relation.Comment = user.Name;
			_relationService.Save(relation);
		}
	}
}