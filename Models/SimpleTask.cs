using CalWebApi.CustomAttributes;
using CalWebApi.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class SimpleTask
    {
        [SwaggerExclude]
        public Guid Id { get; set; }

        [Required]
        public string TaskName { get; set; }

        public RecurrenceType ScheduleRecurrence { get; set; }

        public JArray ScheduleData { get; set; }

        public string Description { get; set; }

        public string CallBack { get; set; }

        public CallbackType ScheduleCallbackType { get; set; }
    }
}
