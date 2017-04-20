﻿using System.Web.Mvc;
using System.Web.Security;
using uCommunity.Core.User;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace uCommunity.Navigation.Web
{
    public abstract class NavigationAuthorizationControllerBase : SurfaceController
    {
        protected virtual string DefaultRedirectUrl { get; } = "/";
        protected virtual string UmbracoRedirectUrl { get; } = "/umbraco";

        protected readonly IIntranetUserService _intranetUserService;
        protected readonly IUserService _userService;

        protected NavigationAuthorizationControllerBase(
            IIntranetUserService intranetUserService,
            IUserService userService)
        {
            _intranetUserService = intranetUserService;
            _userService = userService;
        }

        public virtual ActionResult LoginToUmbraco()
        {
            var currentUser = _intranetUserService.GetCurrentUser();
            if (!currentUser.UmbracoId.HasValue)
            {
                return Redirect(DefaultRedirectUrl);
            }

            var umbracoUser = _userService.GetUserById(currentUser.UmbracoId.Value);
            if (umbracoUser == null
                || umbracoUser.IsLockedOut
                || !umbracoUser.IsApproved)
            {
                return Redirect(DefaultRedirectUrl);
            }

            UmbracoContext.Security.PerformLogin(umbracoUser.Id);

            return Redirect(UmbracoRedirectUrl);
        }

        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}
