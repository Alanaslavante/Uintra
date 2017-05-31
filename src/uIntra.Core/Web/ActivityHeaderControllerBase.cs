﻿using System.Web.Mvc;
using uIntra.Core.Activity;
using Umbraco.Web.Mvc;

namespace uIntra.Core.Web
{
    public abstract class ActivityHeaderControllerBase : SurfaceController
    {
        protected virtual string DetailsHeaderViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityDetailsHeader.cshtml";
        protected virtual string ItemHeaderViewPath { get; } = "~/App_Plugins/Core/Activity/ActivityItemHeader.cshtml";

        public virtual ActionResult ActivityDetailsHeader(IntranetActivityDetailsHeaderViewModel header)
        {
            return PartialView(DetailsHeaderViewPath, header);
        }

        public virtual ActionResult ActivityItemHeader(IntranetActivityItemHeaderViewModel header)
        {
            return PartialView(ItemHeaderViewPath, header);
        }
    }
}