﻿using System.Collections.Generic;

namespace uIntra.Events
{
    public class ComingEventsPanelViewModel
    {
        public string OverviewUrl { get; set; }
        public string Title { get; set; }
        public IEnumerable<ComingEventViewModel> Events { get; set; }
        public bool ShowSeeAllButton { get; set; }
    }
}