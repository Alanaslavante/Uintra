using System;
using Uintra.Core.Activity;

namespace Uintra.Events
{
    public class EventViewModel : IntranetActivityViewModelBase
    {
        public Guid? CreatorId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Media { get; set; }
        public bool CanSubscribe { get; set; }
        public string SubscribeNotes { get; set; }
        public string LocationTitle { get; set; }
    }
}