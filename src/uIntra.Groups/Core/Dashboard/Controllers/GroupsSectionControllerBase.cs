﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using uIntra.Core.Extentions;
using uIntra.Core.User;
using Umbraco.Web.WebApi;

namespace uIntra.Groups.Dashboard
{
    public abstract class GroupsSectionControllerBase : UmbracoAuthorizedApiController
    {
        private readonly IGroupService _groupsService;
        private readonly IGroupLinkProvider _groupLinkProvider;
        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;

        protected GroupsSectionControllerBase(IGroupService groupsService,
            IGroupLinkProvider groupLinkProvider,
            IIntranetUserService<IIntranetUser> intranetUserService)
        {
            _groupsService = groupsService;
            _groupLinkProvider = groupLinkProvider;
            _intranetUserService = intranetUserService;
        }

        public virtual IEnumerable<BackofficeGroupViewModel> GetAll(bool showHidden, string field, Direction direction)
        {
            var groups = showHidden ? _groupsService.GetAll() : _groupsService.GetAllNotHidden();
            var viewModels = groups.Select(s =>
            {
                var viewModel = s.Map<BackofficeGroupViewModel>();
                viewModel.CreatorName = _intranetUserService.Get(s.CreatorId).DisplayedName;
                viewModel.Link = _groupLinkProvider.GetGroupLink(s.Id);
                return viewModel;
            });

            viewModels = Sort(viewModels, field, direction);

            return viewModels;
        }

        [HttpPost]
        public virtual void Hide(Guid groupId, bool hide)
        {
            if (hide)
            {
                _groupsService.Hide(groupId);
            }
            else
            {
                _groupsService.Unhide(groupId);
            }
        }

        protected IEnumerable<BackofficeGroupViewModel> Sort(IEnumerable<BackofficeGroupViewModel> collection, string field, Direction direction)
        {
            var keySelector = GetKeySelector(field);
            switch (direction)
            {
                default:
                    return collection.OrderBy(keySelector);
                case Direction.Desc:
                    return collection.OrderByDescending(keySelector);
            }
        }

        public Func<BackofficeGroupViewModel, object> GetKeySelector(string field)
        {
            switch (field)
            {
                default:
                    return model => model.Title;
                case "createDate":
                    return model => model.CreateDate;
                case "creator":
                    return model => model.CreatorName;
                case "updateDate":
                    return model => model.UpdateDate;
            }
        }
    }
}