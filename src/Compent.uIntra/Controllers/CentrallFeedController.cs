﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compent.uIntra.Core.Activity;
using Compent.uIntra.Core.Feed;
using uIntra.CentralFeed;
using uIntra.CentralFeed.Web;
using uIntra.Core.Activity;
using uIntra.Core.Feed;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Groups;
using uIntra.Subscribe;

namespace Compent.uIntra.Controllers
{

    public class CentralFeedController : CentralFeedControllerBase
    {
        private readonly IIntranetUserService<IGroupMember> _intranetUserService;
        private readonly IGroupFeedService _groupFeedService;
        private readonly IFeedActivityHelper _feedActivityHelper;

        public CentralFeedController(
            ICentralFeedService centralFeedService,
            ICentralFeedContentHelper centralFeedContentHelper,
            IActivitiesServiceFactory activitiesServiceFactory,
            ISubscribeService subscribeService,
            IIntranetUserService<IGroupMember> intranetUserService,
            IIntranetUserContentHelper intranetUserContentHelper,
            IFeedTypeProvider centralFeedTypeProvider,
            ICentralFeedLinkService centralFeedLinkService,
            IGroupFeedService groupFeedService, 
            IFeedActivityHelper feedActivityHelper) 
            : base(centralFeedService,
                  centralFeedContentHelper,
                  activitiesServiceFactory,
                  subscribeService,
                  intranetUserService,
                  intranetUserContentHelper,
                  centralFeedTypeProvider,
                  centralFeedLinkService)
        {
            _intranetUserService = intranetUserService;
            _groupFeedService = groupFeedService;
            _feedActivityHelper = feedActivityHelper;
        }

        protected override IEnumerable<IFeedItem> GetCentralFeedItems(IIntranetType type)
        {
            var groupIds = _intranetUserService.GetCurrentUser().GroupIds;

            var groupFeed = IsTypeForAllActivities(type)
                ? _groupFeedService.GetFeed(groupIds)
                : _groupFeedService.GetFeed(type, groupIds);

            return base.GetCentralFeedItems(type)
                .Concat(groupFeed)
                .OrderByDescending(item => item.PublishDate);
        }

        protected override ActivityFeedOptions GetActivityFeedOptions(Guid id)
        {
            var options = base.GetActivityFeedOptions(id);
            return new ActivityFeedOptionsWithGroups()
            {
                Links = options.Links,
                IsReadOnly = options.IsReadOnly,
                GroupInfo = _feedActivityHelper.GetGroupInfo(id)
            };
        }
    } 
}