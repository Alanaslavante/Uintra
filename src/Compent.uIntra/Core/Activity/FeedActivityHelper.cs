﻿using System;
using uIntra.Core.Extentions;
using uIntra.Groups;

namespace Compent.uIntra.Core.Activity
{
    public class FeedActivityHelper : IFeedActivityHelper
    {
        private readonly IGroupActivityService _groupActivityService;
        private readonly IGroupService _groupService;
        private readonly IGroupLinkProvider _groupLinkProvider;

        public FeedActivityHelper(IGroupActivityService groupActivityService, IGroupService groupService, IGroupLinkProvider groupLinkProvider)
        {
            _groupActivityService = groupActivityService;
            _groupService = groupService;
            _groupLinkProvider = groupLinkProvider;
        }

        public GroupInfo? GetGroupInfo(Guid activityId) => 
            _groupActivityService
                .GetGroupId(activityId)
                .Map(GetInfoForGroup);

        private GroupInfo GetInfoForGroup(Guid groupId)
        {
            var group = _groupService.Get(groupId);
            var groupOverviewUrl = _groupLinkProvider.GetGroupLink(groupId);

            return new GroupInfo(
                title: group.Title,
                url: groupOverviewUrl);
        }
    }
}