﻿namespace uIntra.CentralFeed
{
    public abstract class FeedListModel
    {
        public int TypeId { get; set; }
        public long? Version { get; set; }
        public int Page { get; set; } = 1;
        public FeedFilterStateModel FilterState { get; set; }
    }
}