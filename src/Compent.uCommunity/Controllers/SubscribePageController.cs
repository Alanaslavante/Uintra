﻿using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Compent.uCommunity.Controllers
{
    public class SubscribePageController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            return View(model);
        }
    }
}