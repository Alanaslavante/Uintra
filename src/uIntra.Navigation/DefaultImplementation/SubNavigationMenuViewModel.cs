﻿using System.Collections.Generic;
using System.Linq;

namespace uIntra.Navigation
{
    public class SubNavigationMenuViewModel
    {
        public string Title { get; set; }

        public bool IsTitleHidden { get; set; }

        public IEnumerable<SubNavigationMenuRowModel> Rows { get; set; } = Enumerable.Empty<SubNavigationMenuRowModel>();
        public bool ShowBreadcrumbs { get; set; }


        public MenuItemModel Parent { get; set; }
    }
}
