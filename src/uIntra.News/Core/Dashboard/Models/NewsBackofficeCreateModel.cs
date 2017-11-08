using System;

namespace uIntra.News.Dashboard
{
    public class NewsBackofficeCreateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Media { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? UnpublishDate { get; set; }
        public bool IsHidden { get; set; }
    }
}