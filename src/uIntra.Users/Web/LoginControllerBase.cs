﻿using System.Web.Mvc;
using System.Web.Security;
using uIntra.Core;
using uIntra.Core.Localization;
using Umbraco.Web.Mvc;

namespace uIntra.Users.Web
{
    [AllowAnonymous]
    public abstract class LoginControllerBase : SurfaceController
    {
        protected virtual string LoginViewPath { get; } = "~/App_Plugins/Users/Login/Login.cshtml";
        protected virtual string DefaultRedirectUrl { get; } = "/";

        private readonly ITimezoneOffsetProvider _timezoneOffsetProvider;
        private readonly IIntranetLocalizationService _intranetLocalizationService;

        protected LoginControllerBase(
            ITimezoneOffsetProvider timezoneOffsetProvider,
            IIntranetLocalizationService intranetLocalizationService)
        {
            _timezoneOffsetProvider = timezoneOffsetProvider;
            _intranetLocalizationService = intranetLocalizationService;
        }

        public virtual ActionResult Login()
        {
            var loginStatus = Members.GetCurrentLoginStatus();
            return View(LoginViewPath, loginStatus);
        }

        [HttpPost]
        public virtual ActionResult Login(LoginModelBase model)
        {
            if (!ModelState.IsValid)
            {
                return View(LoginViewPath, model);
            }

            if (!Membership.ValidateUser(model.Login, model.Password))
            {
                ModelState.AddModelError("UserValidation",
                    _intranetLocalizationService.Translate("Login.Validation.UserNotValid"));
                return View(LoginViewPath, model);
            }

            var redirectUrl = model.ReturnUrl ?? DefaultRedirectUrl;

            if (Members.Login(model.Login, model.Password))
            {
                _timezoneOffsetProvider.SetTimezoneOffset(model.ClientTimezoneOffset);
            }
            return Redirect(redirectUrl);
        }

        public virtual void Logout()
        {
            Members.Logout();
        }
    }
}