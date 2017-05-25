﻿using System.Collections.Generic;
using System.Linq;

namespace uIntra.Notification.Core.Configuration
{
    public class ReminderConfiguration
    {
        public IEnumerable<ReminderTypeConfiguration> Configurations { get; set; }

        public ReminderConfiguration()
        {
            Configurations = Enumerable.Empty<ReminderTypeConfiguration>();
        }
    }
}
