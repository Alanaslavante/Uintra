﻿using System.Collections.Generic;

namespace uCommunity.Navigation.DefaultImplementation
{
    public class SystemLinksViewModel
    {
        public string LinksGroupTitle { get; set; }
        public int SortOrder { get; set; }
        public List<SystemLinkItemViewModel> SystemLinks { get; set; }

        public SystemLinksViewModel()
        {
            SystemLinks = new List<SystemLinkItemViewModel>();
        }
    }
}
