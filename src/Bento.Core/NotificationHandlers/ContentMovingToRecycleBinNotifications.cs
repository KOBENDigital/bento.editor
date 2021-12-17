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
		//todo: not 100% sure why we're no just using standard di here?
		//private static readonly IContentService ContentService = DependencyResolver.Current.GetService<IContentService>();
		private readonly IContentService ContentService;
		//private static readonly IRelationService RelationService = DependencyResolver.Current.GetService<IRelationService>();
		private readonly IRelationService RelationService;

		public ContentMovingToRecycleBinNotifications(IContentService contentService, IRelationService relationService)
		{
			ContentService = contentService;
			RelationService = relationService;
		}

		public void Handle(ContentMovingToRecycleBinNotification notification)
		{
			foreach (var trashingEntity in notification.MoveInfoCollection)
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

				notification.CancelOperation(
					new EventMessage("Bento setup",
						$"This content is used in the following places: {string.Join(", ", relations.Select(x => ContentService.GetById(x.ParentId).Name))} (delete failed)",
						EventMessageType.Error));

				break;
			}
		}
	}
}
