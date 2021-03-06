﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Uintra.Core;
using Uintra.Core.Extensions;
using Uintra.Tagging.UserTags;
using Uintra.Tagging.UserTags.Models;
using Umbraco.Web.Mvc;

namespace Uintra.Tagging.Web
{
    public abstract class UserTagsControllerBase : SurfaceController
    {
        protected virtual string TagsPickerViewPath { get; set; } = "~/App_Plugins/UsersTags/Views/TagPicker.cshtml";

        private readonly IUserTagService _tagsService;
        private readonly IUserTagProvider _tagProvider;

        protected UserTagsControllerBase(IUserTagService tagsService, IUserTagProvider tagProvider)
        {
            _tagsService = tagsService;
            _tagProvider = tagProvider;
        }

        public ActionResult TagPicker(Guid? entityId = null)
        {
            var pickerViewModel = GetPickerViewModel(entityId);

            return PartialView(TagsPickerViewPath, pickerViewModel);
        }

        private TagPickerViewModel GetPickerViewModel(Guid? entityId)
        {
            var pickerViewModel = new TagPickerViewModel
            {
                UserTagCollection = _tagProvider.GetAll().Map<IEnumerable<LabeledIdentity<Guid>>>(),
                TagIdsData = entityId.HasValue
                    ? _tagsService.Get(entityId.Value).Select(t => t.Id)
                    : Enumerable.Empty<Guid>()
            };
            return pickerViewModel;
        }
    }
}