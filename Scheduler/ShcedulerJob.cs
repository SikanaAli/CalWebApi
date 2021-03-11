using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Scheduler
{
    [DisallowConcurrentExecution]
    public class ShcedulerJob : IJob
    {
        private readonly ILogger<ShcedulerJob> _logger;

        public ShcedulerJob(ILogger<ShcedulerJob> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
