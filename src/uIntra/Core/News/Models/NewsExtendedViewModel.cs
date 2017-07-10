﻿using uIntra.Comments;
using uIntra.Likes;
using uIntra.News;

namespace uIntra.Core.News.Models
{
    public class NewsExtendedViewModel : NewsViewModel
    {
        public ILikeable LikesInfo { get; set; }
        public ICommentable CommentsInfo { get; set; }
    }
}