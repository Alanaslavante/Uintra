using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using uIntra.Core.Activity;
using uIntra.Core.Media;
using uIntra.Core.ModelBinders;

namespace uIntra.News
{
    public class NewsEditModel : IntranetActivityEditModelBase, IContentWithMediaCreateEditModel
    {
        [Required, AllowHtml]
        public string Description { get; set; }

        [PropertyBinder(typeof(DateTimeBinder))]
        public DateTime PublishDate { get; set; }

        [PropertyBinder(typeof(DateTimeBinder))]
        public DateTime? UnpublishDate { get; set; }

        public string Media { get; set; }

        public int? MediaRootId { get; set; }

        public string NewMedia { get; set; }
    }
}