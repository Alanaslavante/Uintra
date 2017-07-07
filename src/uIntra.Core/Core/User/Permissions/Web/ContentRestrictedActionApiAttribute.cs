﻿using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using uIntra.Core.Activity;
using uIntra.Core.Extentions;
using uIntra.Core.TypeProviders;

namespace uIntra.Core.User.Permissions.Web
{
    public class ContentRestrictedActionApiAttribute : ActionFilterAttribute
    {
        private readonly int _activityTypeId;
        private readonly IntranetActivityActionEnum _action;

        public ContentRestrictedActionApiAttribute(int activityTypeId, IntranetActivityActionEnum action)
        {
            _activityTypeId = activityTypeId;
            _action = action;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var permissionsService = HttpContext.Current.GetService<IPermissionsService>();
            var activityTypeProvider = HttpContext.Current.GetService<IActivityTypeProvider>();

            var isUserHasAccess = permissionsService.IsCurrentUserHasAccess(activityTypeProvider.Get(_activityTypeId), _action);

            if (!isUserHasAccess)
            {
                Deny(filterContext);
            }
        }

        private void Deny(HttpActionContext filterContext)
        {
            filterContext.Response = filterContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User doesn't have permission for this action !");
        }
    }
}
