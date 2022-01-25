using System.Collections.Generic;
using System.Linq;
using Bento.Core.Constants;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace Bento.Core.NotificationHandlers
{
	public class ContentMovingToRecycleBinNotifications : INotificationHandler<ContentMovingToRecycleBinNotification>
	{
		private readonly IContentService _contentService;
		private readonly IRelationService _relationService;

		public ContentMovingToRecycleBinNotifications(IContentService contentService, IRelationService relationService)
		{
			_contentService = contentService;
			_relationService = relationService;
		}

		public void Handle(ContentMovingToRecycleBinNotification notification)
		{
			foreach (var trashingEntity in notification.MoveInfoCollection)
			{
				List<IRelation> relations = _relationService.GetByChildId(trashingEntity.Entity.Id).ToList();

				if (relations.Any() == false)
				{
					continue;
				}

				IEnumerable<IRelationType> relationsTypes = _relationService
					.GetAllRelationTypes(
						relations.Select(x => x.RelationTypeId).ToArray())
					.Where(x => x.Alias == RelationTypes.BentoItemsAlias);

				if (!relationsTypes.Any())
				{
					continue;
				}

				notification.CancelOperation(
					new EventMessage("Bento setup",
						$"This content is used in the following places: {string.Join(", ", relations.Select(x => _contentService.GetById(x.ParentId).Name))} (delete failed)",
						EventMessageType.Error));

				break;
			}
		}
	}
}
