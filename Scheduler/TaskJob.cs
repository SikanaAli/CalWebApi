using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Scheduler
{
    [DisallowConcurrentExecution]
    public class TaskJob : IJob
    {
        //private readonly ILogger<TaskJob> _logger;
        public TaskJob()
        {
           
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"Task Executed @ {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
