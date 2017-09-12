﻿using System;
using uIntra.Core.Links;
using uIntra.Core.TypeProviders;
using uIntra.Core.User;

namespace uIntra.News
{
    public class NewsPreviewViewModel
    {
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public IIntranetUser Creator { get; set; }
        public IIntranetType ActivityType { get; set; }
        public Guid Id { get; set; }
        public ActivityLinks Links { get; set; }
    }
}