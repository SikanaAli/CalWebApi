using CalWebApi.CustomAttributes;
using CalWebApi.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace CalWebApi.Models
{
    public class SimpleTask
    {
        [SwaggerExclude]
        public Guid Id { get; set; }

        [Required]
        public string TaskName { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public RecurrenceType ScheduleRecurrence { get; set; }

        [Required]
        public List<string> ScheduleData { get; set; }

        public string Description { get; set; }
        [Required]
        public string CallBack { get; set; }
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public CallbackType ScheduleCallbackType { get; set; }
    }
}
