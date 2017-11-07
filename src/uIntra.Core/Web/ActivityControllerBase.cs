﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using uIntra.Core.Activity;
using uIntra.Core.Extensions;
using uIntra.Core.Links;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;
using uIntra.Core.User.Permissions;
using Umbraco.Web.Mvc;

namespace uIntra.Core.Web
{
    public abstract class ActivityControllerBase : SurfaceController
    {
        protected virtual string DetailsHeaderViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityDetailsHeader.cshtml";
        protected virtual string ItemHeaderViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityItemHeader.cshtml";
        protected virtual string OwnerEditViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityOwnerEdit.cshtml";
        protected virtual string PinActivityViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityPinView.cshtml";

        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;
        private readonly IPermissionsService _permissionsService;
        private readonly ActivityTypeProvider _activityTypeProvider;

        protected ActivityControllerBase(
            IIntranetUserService<IIntranetUser> intranetUserService,
            IPermissionsService permissionsService,
            ActivityTypeProvider activityTypeProvider)
        {
            _intranetUserService = intranetUserService;
            _permissionsService = permissionsService;
            _activityTypeProvider = activityTypeProvider;
        }

        public virtual ActionResult DetailsHeader(IntranetActivityDetailsHeaderViewModel header)
        {
            return PartialView(DetailsHeaderViewPath, header);
        }

        public virtual ActionResult ItemHeader(object header)
        {
            return PartialView(ItemHeaderViewPath, header);
        }

        public virtual ActionResult OwnerEdit(Guid ownerId, string ownerIdPropertyName, IntranetActivityTypeEnum activityType, IActivityCreateLinks links)
        {
            var model = new IntranetActivityCreatorEditModel
            {
                Owner = _intranetUserService.Get(ownerId),
                OwnerIdPropertyName = ownerIdPropertyName,
                Links = links
            };

            var currentUser = _intranetUserService.GetCurrentUser();
            model.CanEditOwner = _permissionsService.IsRoleHasPermissions(currentUser.Role, PermissionConstants.CanEditCreator);
            if (model.CanEditOwner)
            {
                model.Users = GetUsersWithAccess(activityType, IntranetActivityActionEnum.Create);
            }

            return PartialView(OwnerEditViewPath, model);
        }

        public virtual ActionResult PinActivity(bool isPinned, DateTime? endPinDate)
        {
            return PartialView(PinActivityViewPath,
                new IntranetPinActivityModel
                {
                    IsPinned = isPinned,
                    EndPinDate = endPinDate ?? DateTime.Now
                });
        }

        protected virtual IEnumerable<IIntranetUser> GetUsersWithAccess(IntranetActivityTypeEnum activityType, IntranetActivityActionEnum action)
        {
            var intranetType = _activityTypeProvider.Get(activityType.ToInt());

            var result = _intranetUserService
                .GetAll()
                .Where(user => _permissionsService.IsUserHasAccess(user, intranetType, action))
                .OrderBy(user => user.DisplayedName);

            return result;
        }
    }
}