using System;
using System.Collections.Generic;

namespace uIntra.Navigation
{
    public interface IMyLinksService
    {
        MyLink Get(Guid id);

        MyLink Get(MyLinkDTO model);

        IEnumerable<MyLink> GetMany(IEnumerable<Guid> ids);

        IEnumerable<MyLink> GetMany(Guid userId);

        void Create(MyLinkDTO model);

        void Delete(Guid id);
    }
}