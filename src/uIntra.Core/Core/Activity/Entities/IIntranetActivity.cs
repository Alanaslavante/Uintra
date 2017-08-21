using System;
using System.Collections.Generic;
using uIntra.Core.TypeProviders;

namespace uIntra.Core.Activity
{
    public interface IIntranetActivity
    {
        Guid Id { get; set; }
        IIntranetType Type { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime ModifyDate { get; set; }
        bool IsPinActual { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        bool IsHidden { get; set; }
        bool IsPinned { get; set; }
        DateTime? EndPinDate { get; set; }
    }
}