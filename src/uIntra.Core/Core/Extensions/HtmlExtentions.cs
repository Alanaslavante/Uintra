﻿using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Uintra.Core.Grid;
using Uintra.Core.Links;
using Uintra.Core.Localization;
using Uintra.Core.ModelBinders;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace Uintra.Core.Extensions
{
    public static class HtmlExtensions
    {
        public static string Localize(this HtmlHelper htmlHelper, string key)
        {
            var translationService = HttpContext.Current.GetService<IIntranetLocalizationService>();
            return translationService.Translate(key);
        }

        public static string Localize<T>(this HtmlHelper htmlHelper, T source)
            where T : struct
        {
            var key = $"{typeof(T).Name}.{source}";
            var translationService = HttpContext.Current.GetService<IIntranetLocalizationService>();
            return translationService.Translate(key);
        }

        public static string GetDateFormat(this HtmlHelper htmlHelper)
        {
            var dateTimeFormatProvider = HttpContext.Current.GetService<IDateTimeFormatProvider>();
            return dateTimeFormatProvider.DateFormat;
        }

        public static string GetDateTimePickerFormat(this HtmlHelper htmlHelper)
        {
            var dateTimeFormatProvider = HttpContext.Current.GetService<IDateTimeFormatProvider>();
            return dateTimeFormatProvider.DateTimePickerFormat;
        }

        public static MvcHtmlString PassLinks(this HtmlHelper helper, IActivityLinks links)
        {
            var result = string.Empty;

            result += helper.PassLinks((IActivityCreateLinks)links);
            result += helper.Hidden(LinksBinder.EditFormKey, links.Edit);
            result += helper.Hidden(LinksBinder.DetailsFormKey, links.Details);

            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString PassLinks(this HtmlHelper helper, IActivityCreateLinks links)
        {
            var result = string.Empty;

            result += helper.Hidden(LinksBinder.DetailsNoIdFormKey, links.DetailsNoId);
            result += helper.Hidden(LinksBinder.CreateFormKey, links.Create);
            result += helper.Hidden(LinksBinder.OwnerFormKey, links.Owner);
            result += helper.Hidden(LinksBinder.OverviewFormKey, links.Overview);
            result += helper.Hidden(LinksBinder.FeedFormKey, links.Feed);

            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString RenderGridControl(this HtmlHelper htmlHelper, object control)
        {
            return GridRenderHelper.RenderControl(htmlHelper, (dynamic)control);
        }
    }
}