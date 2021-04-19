using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class ScheduleTaskExample
    {
        public ScheduleTaskAdvanced ScheduleTask { get; set; } = new ScheduleTaskAdvanced
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
