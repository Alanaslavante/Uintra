﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uintra.Core.Activity;
using Uintra.Core.User;
using Uintra.Core.User.Permissions;

namespace Uintra.Core.Extensions
{
    public static class PermissionsExtensions
    {
        public static bool IsRoleHasPermissions(this IPermissionsService service, IRole role, Enum activityType, IntranetActivityActionEnum action)
        {
            var permission = service.GetPermissionFromTypeAndAction(activityType, action);
            return service.IsRoleHasPermissions(role, permission);
        }
    }
}