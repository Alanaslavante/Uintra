﻿using System;
using System.Linq;
using System.Web.Mvc;
using uIntra.Core.ApplicationSettings;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.User;
using uIntra.Notification;
using Umbraco.Web.Mvc;

namespace uIntra.Users.Web
{
    public abstract class ProfileControllerBase : SurfaceController
    {
        protected virtual string ProfileOverViewPath { get; } = "~/App_Plugins/Users/Profile/Overview.cshtml";
        protected virtual string ProfileEditViewPath { get; } = "~/App_Plugins/Users/Profile/Edit.cshtml";

        private readonly IMediaHelper _mediaHelper;
        private readonly IMemberNotifiersSettingsService _memberNotifiersSettingsService;
        private readonly IIntranetUserService<IntranetUser> _intranetUserService;

        protected ProfileControllerBase(
            IMediaHelper mediaHelper,
            IApplicationSettings applicationSettings,
            IIntranetUserService<IntranetUser> intranetUserService,
            IMemberNotifiersSettingsService memberNotifiersSettingsService)
        {
            _mediaHelper = mediaHelper;
            _intranetUserService = intranetUserService;
            _memberNotifiersSettingsService = memberNotifiersSettingsService;
        }

        public virtual ActionResult Overview(Guid? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var user = _intranetUserService.Get(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }

            var result = user.Map<ProfileViewModel>();

            return View(ProfileOverViewPath, result);
        }

        [HttpGet]
        public virtual ActionResult Edit()
        {
            var user = _intranetUserService.GetCurrentUser();
            var result = MapToEditModel(user);

            return View(ProfileEditViewPath, result);
        }

        [HttpPost]
        public virtual ActionResult Edit(ProfileEditModel model)
        {
            var newMedias = _mediaHelper.CreateMedia(model).ToList();

            var updateUser = model.Map<IntranetUserDTO>();
            updateUser.NewMedia = newMedias.Count > 0 ? newMedias.First() : default(int?);

            _intranetUserService.Save(updateUser);

            return RedirectToCurrentUmbracoPage();
        }

        [HttpDelete]
        public virtual void DeletePhoto(string photoPath)
        {
            var user = _intranetUserService.GetCurrentUser();
            _mediaHelper.DeleteMedia(photoPath);

            var updateUser = user.Map<IntranetUserDTO>();
            updateUser.DeleteMedia = true;

            _intranetUserService.Save(updateUser);
        }

        protected virtual ProfileEditModel MapToEditModel(IIntranetUser user)
        {
            var result = user.Map<ProfileEditModel>();
            result.MemberNotifierSettings = _memberNotifiersSettingsService.GetForMember(user.Id);

            FillEditData(result);

            return result;
        }

        protected virtual void FillEditData(ProfileEditModel model)
        {
            var mediaSettings = GetMediaSettings();
            ViewData["AllowedMediaExtentions"] = mediaSettings.AllowedMediaExtentions;
            model.MediaRootId = mediaSettings.MediaRootId;
        }

        public abstract MediaSettings GetMediaSettings();
    }
}
