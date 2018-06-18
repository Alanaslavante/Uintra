﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Uintra.CentralFeed
{
    public class FeedListViewModel
    {
        public Enum Type { get; set; }
        public IEnumerable<FeedItemViewModel> Feed { get; set; } = Enumerable.Empty<FeedItemViewModel>();
        public FeedTabSettings TabSettings { get; set; }
        public bool BlockScrolling { get; set; }
        public bool IsReadOnly { get; set; }
    }
}