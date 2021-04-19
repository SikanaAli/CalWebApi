using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CalWebApi.Helpers;

namespace CalWebApi.Models
{
    public class ScheduleLogsModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TaskID { get; set; }
        [Required]        
        public SimpleTask TaskData { get; set; }
        public string LastExecution { get; set; }
        public ExecutionResult LastExecutionResult { get; set; }

    }
}
