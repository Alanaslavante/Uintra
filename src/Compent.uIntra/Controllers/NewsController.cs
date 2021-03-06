﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Compent.Uintra.Core.Activity.Models;
using Compent.Uintra.Core.Feed;
using Compent.Uintra.Core.News.Entities;
using Compent.Uintra.Core.News.Models;
using Uintra.Core.Extensions;
using Uintra.Core.Links;
using Uintra.Core.Media;
using Uintra.Core.TypeProviders;
using Uintra.Core.User;
using Uintra.Groups;
using Uintra.News;
using Uintra.News.Web;
using Uintra.Search;
using Compent.Uintra.Core.UserTags;
using Uintra.Core;
using Uintra.Groups.Extentions;
using Uintra.Tagging.UserTags;

namespace Compent.Uintra.Controllers
{
    public class NewsController : NewsControllerBase
    {
        protected override string DetailsViewPath => "~/Views/News/DetailsView.cshtml";
        protected override string ItemViewPath => "~/Views/News/ItemView.cshtml";
        protected override string CreateViewPath => "~/Views/News/CreateView.cshtml";
        protected override string EditViewPath => "~/Views/News/EditView.cshtml";

        private readonly INewsService<News> _newsService;
        private readonly IDocumentIndexer _documentIndexer;
        private readonly IGroupActivityService _groupActivityService;
        private readonly UserTagService _userTagService;
        private readonly IActivityTagsHelper _activityTagsHelper;

        public NewsController(
            IIntranetUserService<IIntranetUser> intranetUserService,
            INewsService<News> newsService,
            IMediaHelper mediaHelper,
            IIntranetUserContentProvider intranetUserContentProvider,
            IActivityTypeProvider activityTypeProvider, 
            IDocumentIndexer documentIndexer,
            IGroupActivityService groupActivityService,
            UserTagService userTagService,
            IActivityTagsHelper activityTagsHelper,
            IActivityLinkService activityLinkService,
            IContextTypeProvider contextTypeProvider)
            : base(intranetUserService, newsService, mediaHelper, activityTypeProvider, activityLinkService, contextTypeProvider)
        {
            _newsService = newsService;
            _documentIndexer = documentIndexer;
            _groupActivityService = groupActivityService;
            _userTagService = userTagService;
            _activityTagsHelper = activityTagsHelper;
        }

        public ActionResult FeedItem(News item, ActivityFeedOptionsWithGroups options)
        {
            var extendedModel = GetItemViewModel(item, options);
            AddEntityIdentityForContext(item.Id);
            return PartialView(ItemViewPath, extendedModel);
        }



        private NewsExtendedItemViewModel GetItemViewModel(News item, ActivityFeedOptionsWithGroups options)
        {
            var model = GetItemViewModel(item, options.Links);
            var extendedModel = model.Map<NewsExtendedItemViewModel>();

            extendedModel.HeaderInfo = model.HeaderInfo.Map<ExtendedItemHeaderViewModel>();
            extendedModel.HeaderInfo.GroupInfo = options.GroupInfo;

            extendedModel.LikesInfo = item;
            extendedModel.LikesInfo.IsReadOnly = options.IsReadOnly;
            extendedModel.IsReadOnly = options.IsReadOnly;
            return extendedModel;
        }

        public ActionResult PreviewItem(News item, ActivityLinks links)
        {
            NewsPreviewViewModel viewModel = GetPreviewViewModel(item, links);
            AddEntityIdentityForContext(item.Id);
            return PartialView(PreviewItemViewPath, viewModel);
        }

        [HttpPost]
        public ActionResult EditExtended(NewsExtendedEditModel editModel)
        {
            return Edit(editModel);
        }

        [HttpPost]
        public ActionResult CreateExtended(NewsExtendedCreateModel createModel)
        {
            return Create(createModel);
        }

        protected override NewsEditModel GetEditViewModel(NewsBase news, ActivityLinks links)
        {
            var extendedModel = base.GetEditViewModel(news, links).Map<NewsExtendedEditModel>();
            //extendedModel.TagIdsData = _userTagService.GetRelatedTags(extendedModel.Id).JoinToString();
            return extendedModel;
        }

        protected override NewsCreateModel GetCreateModel(IActivityCreateLinks links)
        {
            var extendedModel = base.GetCreateModel(links).Map<NewsExtendedCreateModel>();
            return extendedModel;
        }

        protected override void OnNewsEdited(NewsBase news, NewsEditModel model)
        {
            if (model is NewsExtendedEditModel extendedModel)
            {
                _activityTagsHelper.ReplaceTags(news.Id, extendedModel.TagIdsData);
            }
        }

        protected override NewsViewModel GetViewModel(NewsBase news)
        {
            var extendedNews = (News)news;
            var extendedModel = base.GetViewModel(news).Map<NewsExtendedViewModel>();
            extendedModel = Mapper.Map(extendedNews, extendedModel);
            
            return extendedModel;
        }

        protected override void DeleteMedia(IEnumerable<int> mediaIds)
        {
            base.DeleteMedia(mediaIds);
            _documentIndexer.DeleteFromIndex(mediaIds);
        }

        protected override void OnNewsCreated(Guid activityId, NewsCreateModel model)
        {
            var groupId = Request.QueryString.GetGroupId();
            if (groupId.HasValue)
            {
                _groupActivityService.AddRelation(groupId.Value, activityId);
                var news = _newsService.Get(activityId);
                news.GroupId = groupId;
            }
            if (model is NewsExtendedCreateModel extendedModel)
            {
                _activityTagsHelper.ReplaceTags(activityId, extendedModel.TagIdsData);
            }
        }
    }
}