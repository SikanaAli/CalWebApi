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
            ScheduleTaskAdvanced data = new ScheduleTaskAdvanced();
            if (context.JobDetail.JobDataMap.GetString("type") != "simple")
            {
                data = JsonConvert.DeserializeObject<ScheduleTaskAdvanced>(context.JobDetail.JobDataMap.GetString("data"));
            }
            else
            {
                SimpleTask temp = JsonConvert.DeserializeObject<SimpleTask>(context.JobDetail.JobDataMap.GetString("data"));
                
                data.Discription = temp.Description;
                data.Id = temp.Id;
                data.TaskName = temp.TaskName;
                data.DateCreated = temp.DateCreated;
                data.CallBackUrl = temp.CallBack;

            }

            try
            {
                WebRequest request = WebRequest.Create(data.CallBackUrl);
                WebResponse response = request.GetResponse();
                var responseCode = ((HttpWebResponse)response).StatusDescription;
                response.Close();
                Console.WriteLine($"Task Executed @ {DateTime.Now} with Response {responseCode} {data.TaskName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.CompletedTask;
        }
    }
}
