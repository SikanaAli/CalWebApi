using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.CustomAttributes;
namespace CalWebApi.Models
{
    public class ScheduleTaskAdvanced
    {
        [SwaggerExclude]
        public Guid Id { get; set; }

        [Required]
        public string TaskName { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public string CronExpression { get; set; }

        [Required]
        public string CallBackUrl { get; set; }

        public string Discription { get; set; }
    }
}
