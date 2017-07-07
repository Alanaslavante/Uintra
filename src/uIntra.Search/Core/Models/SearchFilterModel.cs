﻿using System.Collections.Generic;

namespace uIntra.Search
{
    public class SearchFilterModel
    {
        public string Query { get; set; }
        public int Page { get; set; }
        public IList<int> Types { get; set; } = new List<int>();
    }
}
