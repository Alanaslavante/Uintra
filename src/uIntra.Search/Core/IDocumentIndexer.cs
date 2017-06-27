﻿using System.Collections.Generic;

namespace uIntra.Search.Core
{
    public interface IDocumentIndexer
    {
        void Index(int id);
        void Index(IEnumerable<int> ids);

        void DeleteFromIndex(int id);
        void DeleteFromIndex(IEnumerable<int> ids);
    }
}