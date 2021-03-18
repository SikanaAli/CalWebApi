﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class ScheduledTasks
    {
        public string Group { get; set; }
        public string TaskName { get; set; }
        public string TaskKey { get; set; }
        public string TriggerKey { get; set; }
        public DateTimeOffset? NextFireTime { get; set; }
        public DateTimeOffset? PreviousFireTime { get; set; }
    }
}