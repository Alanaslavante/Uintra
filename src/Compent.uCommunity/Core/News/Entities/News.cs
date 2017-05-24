﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using uCommunity.CentralFeed;
using uCommunity.Comments;
using uCommunity.Likes;
using uCommunity.News;
using uCommunity.Subscribe;
using uCommunity.Tagging;

namespace Compent.uCommunity.Core.News.Entities
{
    public class News : NewsBase, ICentralFeedItem, ICommentable, ILikeable, ISubscribable, IHaveTags
    {
        [JsonIgnore]
        public DateTime SortDate => PublishDate;
        [JsonIgnore]
        public IEnumerable<LikeModel> Likes { get; set; }
        [JsonIgnore]
        public IEnumerable<Comment> Comments { get; set; }
        [JsonIgnore]
        public IEnumerable<global::uCommunity.Subscribe.Subscribe> Subscribers { get; set; }
        [JsonIgnore]
        public IEnumerable<Tag> Tags { get; set; }
    }
}