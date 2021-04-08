using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class ScheduleTaskExample
    {
        public ScheduleTask ScheduleTask { get; set; } = new ScheduleTask
        {
            CallBackUrl = "http://mysite.com/",
            TaskName = "Do Somthing",
            CronExpression = "5 * * * *",
            Discription = "My task does somthing",
            DateCreated = new DateTime(),
            Id = Guid.NewGuid(),
        };
    }
}
