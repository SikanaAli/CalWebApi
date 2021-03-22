using CalWebApi.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

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
            ScheduleTask data = JsonConvert.DeserializeObject<ScheduleTask>(context.JobDetail.JobDataMap.GetString("data"));

            try
            {
                WebRequest request = WebRequest.Create(data.CallBackUrl);
                WebResponse response = request.GetResponse();
                var responseCode = ((HttpWebResponse)response).StatusDescription;
                response.Close();
                Console.WriteLine($"Task Executed @ {DateTime.Now} with Response {responseCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.CompletedTask;
        }
    }
}
