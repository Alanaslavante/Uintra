﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using uCommunity.CentralFeed;
//using uCommunity.CentralFeed.Entities;
//using uCommunity.Comments;
//using uCommunity.Core.Activity;
//using uCommunity.Core.Activity.Sql;
//using uCommunity.Core.Caching;
//using uCommunity.Core.Media;
//using uCommunity.Core.User;
//using uCommunity.Events;
//using uCommunity.Likes;
//using uCommunity.Subscribe;
//using Umbraco.Core.Models;
//using Umbraco.Web;

//namespace Compent.uCommunity.Core.Events
//{
//    public class EventsService : IntranetActivityItemServiceBase<EventBase, Event>,
//        IEventsService<EventBase, Event>,
//        ICentralFeedItemService,
//        ISubscribableService,
//        ILikeableService,
//        ICommentableService
//    {
//        private readonly UmbracoHelper _umbracoHelper;
//        private readonly IIntranetUserService _intranetUserService;
//        private readonly ICommentsService _commentsService;
//        private readonly ILikesService _likesService;
//        private readonly ISubscribeService _subscribeService;

//        public EventsService(UmbracoHelper umbracoHelper,
//            IIntranetActivityService intranetActivityService,
//            IMemoryCacheService memoryCacheService,
//            IIntranetUserService intranetUserService,
//            ICommentsService commentsService,
//            ILikesService likesService,
//            ISubscribeService subscribeService)
//            : base(intranetActivityService, memoryCacheService)
//        {
//            _umbracoHelper = umbracoHelper;
//            _intranetUserService = intranetUserService;
//            _commentsService = commentsService;
//            _likesService = likesService;
//            _subscribeService = subscribeService;
//        }

//        protected override List<string> OverviewXPath { get; }
//        public override IntranetActivityTypeEnum ActivityType => IntranetActivityTypeEnum.Events;

//        public override IPublishedContent GetOverviewPage()
//        {
//            return _umbracoHelper.TypedContent(1163);
//        }

//        public override IPublishedContent GetDetailsPage()
//        {
//            return _umbracoHelper.TypedContent(1165);
//        }

//        public override IPublishedContent GetCreatePage()
//        {
//            return _umbracoHelper.TypedContent(1164);
//        }

//        public override IPublishedContent GetEditPage()
//        {
//            return _umbracoHelper.TypedContent(1166);
//        }

//        public IEnumerable<Event> GetPastEvents()
//        {
//            throw new NotImplementedException();
//        }

//        public void Hide(Guid id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool CanEditSubscribe(EventBase activity)
//        {
//            return true;
//        }

//        public bool CanSubscribe(EventBase activity)
//        {
//            return true;
//        }

//        public bool HasSubscribers(EventBase activity)
//        {
//            throw new NotImplementedException();
//        }

//        public MediaSettings GetMediaSettings()
//        {
//            return new MediaSettings
//            {
//                MediaRootId = 1099
//            };
//        }

//        public CentralFeedSettings GetCentralFeedSettings()
//        {
//            return new CentralFeedSettings
//            {
//                Type = ActivityType,
//                Controller = "Events",
//                OverviewPage = GetOverviewPage(),
//                CreatePage = GetCreatePage()
//            };
//        }

//        public override bool CanEdit(EventBase activity)
//        {
//            return true;
//        }

//        public ICentralFeedItem GetItem(Guid activityId)
//        {
//            var item = Get(activityId);
//            return item;
//        }

//        public IEnumerable<ICentralFeedItem> GetItems()
//        {
//            var items = GetManyActual().OrderByDescending(i => i.PublishDate);
//            return items;
//        }

//        protected override Event FillPropertiesOnGet(IntranetActivityEntity entity)
//        {
//            var activity = base.FillPropertiesOnGet(entity);
//            _subscribeService.FillSubscribers(activity);
//            _intranetUserService.FillCreator(activity);
//            _commentsService.FillComments(activity);
//            _likesService.FillLikes(activity);

//            return activity;
//        }

//        public global::uCommunity.Subscribe.Subscribe Subscribe(Guid userId, Guid activityId)
//        {
//            throw new NotImplementedException();
//        }

//        public void UnSubscribe(Guid userId, Guid activityId)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateNotification(Guid id, bool value)
//        {
//            throw new NotImplementedException();
//        }

//        public void Add(Guid userId, Guid activityId)
//        {
//            _likesService.Add(userId, activityId);
//            FillCache(activityId);
//        }

//        public void Remove(Guid userId, Guid activityId)
//        {
//            _likesService.Remove(userId, activityId);
//            FillCache(activityId);
//        }

//        public IEnumerable<LikeModel> GetLikes(Guid activityId)
//        {
//            return Get(activityId).Likes;
//        }

//        public void CreateComment(Guid userId, Guid activityId, string text, Guid? parentId)
//        {
//            _commentsService.Create(userId, activityId, text, parentId);
//            FillCache(activityId);
//        }

//        public void UpdateComment(Guid id, string text)
//        {
//            var comment = _commentsService.Update(id, text);
//            FillCache(comment.ActivityId);
//        }

//        public void DeleteComment(Guid id)
//        {
//            var comment = _commentsService.Get(id);
//            _commentsService.Delete(id);
//            FillCache(comment.ActivityId);
//        }

//        public ICommentable GetCommentsInfo(Guid activityId)
//        {
//            return Get(activityId);
//        }
//    }
//}