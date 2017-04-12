﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using uCommunity.Core.Activity;
using uCommunity.Core.User;
using uCommunity.Core.User.Permissions;

namespace uCommunity.Core.Extentions
{
    public static class PermissionsExtentions
    {
        public static bool IsRoleHasPermissions(this IPermissionsService service, IntranetRolesEnum role,
            IEnumerable<KeyValuePair<IntranetActivityTypeEnum, IntranetActivityActionEnum>> collection)
        {
            var permissions = collection.Select(s => service.GetPermissionFromTypeAndAction(s.Key, s.Value)).ToArray();
            return service.IsRoleHasPermissions(role, permissions);
        }

        public static bool IsRoleHasPermissions(this IPermissionsService service, IntranetRolesEnum role, IntranetActivityTypeEnum activityType, IntranetActivityActionEnum action)
        {
            var permission = service.GetPermissionFromTypeAndAction(activityType, action);
            return service.IsRoleHasPermissions(role, permission);
        }


        public static bool IsCurrentUserHasPermission(params string[] permissions)
        {
            var permissionsService = HttpContext.Current.GetService<IPermissionsService>();
            return IsCurrentUserHasPermission(permissionsService, permissions);
        }

        public static bool IsCurrentUserHasPermission(IntranetActivityTypeEnum activityType, IntranetActivityActionEnum action)
        {
            var permissionsService = HttpContext.Current.GetService<IPermissionsService>();
            var permission = permissionsService.GetPermissionFromTypeAndAction(activityType, action);
            return IsCurrentUserHasPermission(permissionsService, permission);
        }

        public static bool IsCurrentUserHasPermission(IEnumerable<KeyValuePair<IntranetActivityTypeEnum, IntranetActivityActionEnum>> collection)
        {
            var permissionsService = HttpContext.Current.GetService<IPermissionsService>();
            var permissions = collection.Select(s => permissionsService.GetPermissionFromTypeAndAction(s.Key, s.Value)).ToArray();
            return IsCurrentUserHasPermission(permissionsService, permissions);
        }

        private static bool IsCurrentUserHasPermission(IPermissionsService service, params string[] permissions)
        {
            var userService = HttpContext.Current.GetService<IIntranetUserService>();
            var currentUser = userService.GetCurrentUser();

            if (currentUser == null)
            {
                return false;
            }

            var isAllowed = service.IsRoleHasPermissions(currentUser.Role, permissions);
            return isAllowed;
        }
    }
}