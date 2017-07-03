﻿using System;
using System.Collections.Generic;
using System.Linq;
using uIntra.Bulletins;
using uIntra.CentralFeed;
using uIntra.Comments;
using uIntra.Core.Activity;
using uIntra.Core.Caching;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Core.User.Permissions;
using uIntra.Likes;
using uIntra.Notification;
using uIntra.Notification.Base;
using uIntra.Notification.Configuration;
using uIntra.Subscribe;
using uIntra.Users;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Compent.uIntra.Core.Bulletins
{
    public class BulletinsService : IntranetActivityService<Bulletin>,
        IBulletinsService<Bulletin>,
        ICentralFeedItemService,
        ICommentableService,
        ILikeableService,
        INotifyableService
    {
        private readonly IIntranetUserService<IntranetUser> _intranetUserService;
        private readonly ICommentsService _commentsService;
        private readonly ILikesService _likesService;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ISubscribeService _subscribeService;
        private readonly IPermissionsService _permissionsService;
        private readonly INotificationsService _notificationService;
        private readonly IActivityTypeProvider _activityTypeProvider;
        private readonly ICentralFeedTypeProvider _centralFeedTypeProvider;

        public BulletinsService(
            IIntranetActivityRepository intranetActivityRepository,
            ICacheService cacheService,
            IIntranetUserService<IntranetUser> intranetUserService,
            ICommentsService commentsService,
            ILikesService likesService,
            ISubscribeService subscribeService,
            UmbracoHelper umbracoHelper,
            IPermissionsService permissionsService,
            INotificationsService notificationService, IActivityTypeProvider activityTypeProvider, 
            ICentralFeedTypeProvider centralFeedTypeProvider)
            : base(intranetActivityRepository, cacheService, activityTypeProvider)
        {
            _intranetUserService = intranetUserService;
            _commentsService = commentsService;
            _likesService = likesService;
            _umbracoHelper = umbracoHelper;
            _permissionsService = permissionsService;
            _subscribeService = subscribeService;
            _notificationService = notificationService;
            _activityTypeProvider = activityTypeProvider;
            _centralFeedTypeProvider = centralFeedTypeProvider;
        }

        public MediaSettings GetMediaSettings()
        {
            return new MediaSettings
            {
                MediaRootId = 1249
            };
        }

        public override IPublishedContent GetOverviewPage()
        {
            return _umbracoHelper.TypedContent(1245);
        }

        public override IPublishedContent GetDetailsPage()
        {
            return _umbracoHelper.TypedContent(1333);
        }

        public override IPublishedContent GetCreatePage()
        {
            throw new NotImplementedException();
        }

        public override IPublishedContent GetEditPage()
        {
            return _umbracoHelper.TypedContent(1334);
        }
        public override IIntranetType ActivityType => _activityTypeProvider.Get(IntranetActivityTypeEnum.Bulletins.ToInt());

        public override bool CanEdit(IIntranetActivity cached)
        {
            var result = CanPerform(cached, IntranetActivityActionEnum.Edit);
            return result;
        }

        public bool CanDelete(IIntranetActivity cached)
        {
            var result = CanPerform(cached, IntranetActivityActionEnum.Delete);
            return result;
        }

        public CentralFeedSettings GetCentralFeedSettings()
        {
            return new CentralFeedSettings
            {
                Type = _centralFeedTypeProvider.Get(CentralFeedTypeEnum.Bulletins.ToInt()),
                Controller = "Bulletins",                
                HasPinnedFilter = false,
                HasSubscribersFilter = false
            };
        }

        public bool IsActual(Bulletin activity)
        {
            return base.IsActual(activity) && activity.PublishDate.Date <= DateTime.Now.Date;
        }

        public ICentralFeedItem GetItem(Guid activityId)
        {
            var bulletin = Get(activityId);
            return bulletin;
        }

        public IEnumerable<ICentralFeedItem> GetItems()
        {
            var items = GetManyActual().OrderByDescending(i => i.PublishDate);
            return items;
        }

        protected override void MapBeforeCache(IList<IIntranetActivity> cached)
        {
            foreach (var activity in cached)
            {
                var entity = activity as Bulletin;
                _subscribeService.FillSubscribers(entity);
                _commentsService.FillComments(entity);
                _likesService.FillLikes(entity);
            }
        }

        public Comment CreateComment(Guid userId, Guid activityId, string text, Guid? parentId)
        {
            var comment = _commentsService.Create(userId, activityId, text, parentId);
            UpdateCachedEntity(comment.ActivityId);
            return comment;
        }

        public void UpdateComment(Guid id, string text)
        {
            var comment = _commentsService.Update(id, text);
            UpdateCachedEntity(comment.ActivityId);
        }

        public void DeleteComment(Guid id)
        {
            var comment = _commentsService.Get(id);
            _commentsService.Delete(id);
            UpdateCachedEntity(comment.ActivityId);
        }

        public ICommentable GetCommentsInfo(Guid activityId)
        {
            return Get(activityId);
        }

        public ILikeable Add(Guid userId, Guid activityId)
        {
            _likesService.Add(userId, activityId);
            return UpdateCachedEntity(activityId);
        }

        public ILikeable Remove(Guid userId, Guid activityId)
        {
            _likesService.Remove(userId, activityId);
            return UpdateCachedEntity(activityId);
        }

        public IEnumerable<LikeModel> GetLikes(Guid activityId)
        {
            return Get(activityId).Likes;
        }

        public void Notify(Guid entityId, IIntranetType notificationType)
        {
            var notifierData = GetNotifierData(entityId, notificationType);
            if (notifierData != null)
            {
                _notificationService.ProcessNotification(notifierData);
            }
        }

        public override IPublishedContent GetOverviewPage(IPublishedContent currentPage)
        {
            return GetOverviewPage();
        }

        public override IPublishedContent GetDetailsPage(IPublishedContent currentPage)
        {
            return GetDetailsPage();
        }

        public override IPublishedContent GetCreatePage(IPublishedContent currentPage)
        {
            return GetCreatePage();
        }

        public override IPublishedContent GetEditPage(IPublishedContent currentPage)
        {
            return GetEditPage();
        }

        private NotifierData GetNotifierData(Guid entityId, IIntranetType notificationType)
        {
            Bulletin bulletinsEntity;
            var currentUser = _intranetUserService.GetCurrentUser();
            var data = new NotifierData
            {
                NotificationType = notificationType
            };

            switch (notificationType.Id)
            {
                case (int) NotificationTypeEnum.ActivityLikeAdded:
                {
                    bulletinsEntity = Get(entityId);
                    data.ReceiverIds = bulletinsEntity.CreatorId.ToEnumerableOfOne();
                    data.Value = new LikesNotifierDataModel
                    {
                        Title = bulletinsEntity.Title,
                        ActivityType = ActivityType,
                        NotifierId = currentUser.Id
                    };
                }
                    break;
                case (int) NotificationTypeEnum.CommentAdded:
                case (int) NotificationTypeEnum.CommentEdited:
                {
                    var comment = _commentsService.Get(entityId);
                    bulletinsEntity = Get(comment.ActivityId);
                    data.ReceiverIds = bulletinsEntity.CreatorId.ToEnumerableOfOne();
                    data.Value = new CommentNotifierDataModel
                    {
                        ActivityType = ActivityType,
                        NotifierId = comment.UserId,
                        Title = bulletinsEntity.Title,
                        Url = GetUrlWithComment(bulletinsEntity.Id, comment.Id)
                    };
                }
                    break;
                case (int) NotificationTypeEnum.CommentReplyed:
                {
                    var comment = _commentsService.Get(entityId);
                    bulletinsEntity = Get(comment.ActivityId);
                    data.ReceiverIds = comment.UserId.ToEnumerableOfOne();
                    data.Value = new CommentNotifierDataModel
                    {
                        ActivityType = ActivityType,
                        NotifierId = currentUser.Id,
                        Title = bulletinsEntity.Title,
                        Url = GetUrlWithComment(bulletinsEntity.Id, comment.Id),
                        CommentId = comment.Id
                    };
                }
                    break;
                default:
                    return null;
            }
            return data;
        }

        private string GetUrlWithComment(Guid bulletinId, Guid commentId)
        {
            return $"{GetDetailsPage().Url.UrlWithQueryString("id", bulletinId)}#{_commentsService.GetCommentViewId(commentId)}";
        }

        public ILikeable AddLike(Guid userId, Guid activityId)
        {
            _likesService.Add(userId, activityId);
            return UpdateCachedEntity(activityId);
        }

        public ILikeable RemoveLike(Guid userId, Guid activityId)
        {
            _likesService.Remove(userId, activityId);
            return UpdateCachedEntity(activityId);
        }

        private bool CanPerform(IIntranetActivity cached, IntranetActivityActionEnum action)
        {
            var currentUser = _intranetUserService.GetCurrentUser();

            var isWebmater = _permissionsService.IsUserWebmaster(currentUser);
            if (isWebmater)
            {
                return true;
            }

            var creatorId = Get(cached.Id).CreatorId;
            var isCreator = creatorId == currentUser.Id;

            var isUserHasPermissions = _permissionsService.IsRoleHasPermissions(currentUser.Role, ActivityType, action);
            return isCreator && isUserHasPermissions;
        }
    }
}