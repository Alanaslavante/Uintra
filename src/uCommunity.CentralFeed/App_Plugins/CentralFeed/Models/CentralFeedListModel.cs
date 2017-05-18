﻿using uCommunity.CentralFeed.Enums;
using uCommunity.Core.Activity;

namespace uCommunity.CentralFeed.Models
{
    public class CentralFeedListModel
    {
        public CentralFeedTypeEnum Type { get; set; }
        public bool? ShowSubscribed { get; set; }
        public long? Version { get; set; }
        public int Page { get; set; } = 1;
    }
}