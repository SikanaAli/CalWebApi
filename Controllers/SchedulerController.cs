using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
using Quartz;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using CalWebApi.Scheduler;
using Microsoft.Extensions.Logging;
using Quartz.Impl.Matchers;
using CalWebApi.Helpers;

namespace CalWebApi.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class SchedulerController : ControllerBase
    {
        //Readonly Vars
        private readonly IScheduler scheduler;
        private readonly ILogger<TaskJob> logger;
        //Contructor
        public SchedulerController(IScheduler _scheduler, ILogger<TaskJob> _logger)
        {
            scheduler = _scheduler;
            logger = _logger;
        }

        // GET: api/v1/Scheduler
        [HttpGet]
        public async Task<Array> Get()
        {
            return await GetScheduledTasks();
        }

        [HttpPost]
        [Route("SimpleTask")]
        [ApiVersion("1.1")]
        [Consumes(contentType:"application/json")]
        public async Task<IActionResult> ScheduleSimpleTask([FromBody]SimpleTask task)
        {
            //SimpleTask task = JsonConvert.DeserializeObject<SimpleTask>(jtask.ToString() ,new Newtonsoft.Json.Converters.StringEnumConverter());

            if (!this.ModelState.IsValid)
                return BadRequest(modelState: ModelState);

            task.Id = Guid.NewGuid();

            switch (task.ScheduleRecurrence)
            {
                case RecurrenceType.Minutes:
                    if (!isValidScheduleData(task))
                    {
                        return BadRequest("Invalid Minute Value");
                    }
                    break;
                case RecurrenceType.Hourly:
                    if (!isValidScheduleData(task))
                    {
                        return BadRequest("Invalid Start At/Hourly value");
                    }
                    break;
                case RecurrenceType.Daily:
                    if (!isValidScheduleData(task))
                    {
                        return BadRequest("Internal Server Error");
                    }
                    break;
                case RecurrenceType.Weekly:
                    break;
                case RecurrenceType.Monthly:
                    break;
                case RecurrenceType.Yearly:
                    break;
                default:
                    break;
            }

            IJobDetail simpleJob = CreateSimpleJob(task);
            ITrigger simpleTrigger = CreateSimpleTrigger(task);

            if(task.ScheduleRecurrence  == RecurrenceType.Hourly && task.ScheduleData.ContainsKey("startat"))
            {
                await scheduler.ScheduleJob(simpleJob, (ISimpleTrigger)simpleTrigger);
                return Ok("Task Scheduled");
            }


            await scheduler.ScheduleJob(simpleJob, simpleTrigger);
            
            return Ok("Task Scheduled");
        }

        /// <summary>
        /// Schedule a new task
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Task
        ///     {
        ///        "callBackUrl": "http://mysite.com/backup/logs", 
        ///        "cronExpression": "5 * * * *",
        ///        "dateCreated": "2021-03-16T03:23:46.217Z",
        ///        "discription": "This is my task that does somthing",
        ///        "taskName": "Do Somthing"
        ///     }
        ///
        /// </remarks>
        /// <returns>Status code and messages wherer necessary</returns>
        /// <param name="task"></param>
        /// <response code="201">Reruns 201 status and 'Task Schdeuled'</response>
        /// <response code="400">If the json is not stuctured well or invalid data</response>  
        [HttpPost]
        [Route("Task")]
        [ApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(JObject),typeof(ScheduleTaskExample))]
        public async Task<IActionResult> ScheduleTask([FromBody]ScheduleTaskAdvanced task)
        {
            if (ModelState.IsValid)
            {
                if (CronExpression.IsValidExpression(task.CronExpression))
                {
                    if (scheduler.IsStarted)
                    {
                        //await scheduler.Start();
                        
                        var metadata = new TaskMetadata(Guid.NewGuid(), typeof(TaskJob),task.TaskName, task.CronExpression);
                        var Job = CreateAdvancedJob(metadata,task);
                        var trigger = CreateTrigger(metadata);

                        
                        await scheduler.ScheduleJob(Job, trigger);
                        return Created(Request.Path, "Task Scheduled");
                    }
                    else
                    {
                        return BadRequest("Scheduler NotStarted");
                    }
                }
                else
                {
                    return BadRequest("Invalid Cron");
                }
            }
            else
            {

                var error = new JObject();
                error.Add("StatusCode", (int)HttpStatusCode.BadRequest);
                error.Add("error", JObject.FromObject(ModelState).ToString());

                return BadRequest(error.ToString());
            }
        }

        /// <summary>
        /// Unschedule a task
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Unschedule
        ///     
        ///     {
        ///         taskid:task_id_goes_here
        ///     }
        ///     
        ///
        /// </remarks>
        /// <returns>Status code and messages where necessary</returns>
        /// <param name="taskids"></param>
        /// <response code="200">Reruns 201 status and 'Task Schdeuled'</response>
        /// <response code="400">If the json is not stuctured well or invalid data</response>  
        [HttpPut]
        [ApiVersion("1.0")]
        [Route("Pause")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(JObject), typeof(ScheduleTaskExample))]
        public async Task<IActionResult> PauseTask([FromBody]TaskIdFromBody taskids)
        {
            try
            {
               
                foreach (var taskKey in taskids.TaskIDs)
                {
                    await scheduler.PauseJob(new JobKey(Guid.Parse(taskKey).ToString()));
                }
                return Ok();
            }
            catch (Exception)
            {

                return BadRequest("Invalid ID");
            }

            
            
        }


        /// <summary>
        /// Reschedule a task
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Reschedule
        ///     
        ///     {
        ///         taskid:[task_ids_goes_here]
        ///     }
        ///     
        ///
        /// </remarks>
        /// <returns>Status code and messages where necessary</returns>
        /// <param name="tasks"></param>
        /// <response code="200">Reruns 201 status and 'Task Schdeuled'</response>
        /// <response code="400">If the json is not stuctured well or invalid data</response>  
        [HttpPut]
        [ApiVersion("1.0")]
        [Route("Resume")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RescheduleTask([FromBody]TaskIdFromBody tasks)
        {
            try
            {
                foreach (var id in tasks.TaskIDs)
                {
                    JobKey key = new JobKey(Guid.Parse(id).ToString());
                    await scheduler.ResumeJob(key);
                }
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Delete All Tasks
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAll")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> DeleteAllJobs()
        {

            IList<string> TaskGroups = (IList<String>)await scheduler.GetJobGroupNames();
            List<JobKey> jobKeys = new List<JobKey>();

            foreach (var group in TaskGroups)
            {
                var GM = GroupMatcher<JobKey>.GroupContains(group);
                var TKs = await scheduler.GetJobKeys(GM);
                foreach (var tk in TKs)
                {
                    jobKeys.Add(tk);
                }
            }
            var done = await scheduler.DeleteJobs(jobKeys);
            return Ok($"Was OK {done}");
        }

        

        /// <summary>
        /// Deletes Multiple Tasks
        /// </summary>
        /// <param name="idsFromBody"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteTasks")]
        public async Task<IActionResult> DeleteTasks([FromBody]DeleteTaskIdsFromBody idsFromBody)
        {
            JObject responseObj = new JObject();
            try
            {
                var canProceed = await CheckIfKeyExists(idsFromBody.TaskIds);
                
                if (canProceed)
                {
                    List<JobKey> jKeys = new List<JobKey>();
                    idsFromBody.TaskIds.ForEach((id) =>
                    {
                        jKeys.Add(new JobKey(Guid.Parse(id).ToString()));
                    });

                    var result = await scheduler.DeleteJobs(jKeys);
                    if (result)
                    {
                        responseObj.Add("response", "Task(s) suceefuly deleted");
                        return Ok( responseObj.ToString());
                    }
                    
                }
                responseObj.Add("response", "an error has occured");
                return BadRequest(responseObj.ToString());
            }
            catch (Exception)
            {
                responseObj.Add("response", "An Error ocured whith on of the values");
                return BadRequest(responseObj.ToString());
            };
        }

        //Task Creation Methods
        private ITrigger CreateTrigger(TaskMetadata jobMetadata)
        {
            return TriggerBuilder.Create()
            .WithIdentity(jobMetadata.TaskId.ToString())
            .WithCronSchedule(jobMetadata.CronExpression)
            .WithDescription($"{jobMetadata.TaskName}")
            .Build();
        }


        private IJobDetail CreateAdvancedJob(TaskMetadata jobMetadata, ScheduleTaskAdvanced task)
        {
            return JobBuilder
            .Create(jobMetadata.TaskType)
            .UsingJobData("type","advanced")
            .UsingJobData("data",JObject.FromObject(task).ToString())
            .WithIdentity(jobMetadata.TaskId.ToString())
            .WithDescription(task.Discription)
            .StoreDurably(true)
            .Build();
        }

        private IJobDetail CreateSimpleJob(SimpleTask task)
        {
            return JobBuilder
                .Create(jobType: typeof(TaskJob))
                .StoreDurably(durability: true)
                .WithIdentity(task.Id.ToString())
                .WithDescription(task.Description)
                .UsingJobData("type","simple")
                .UsingJobData("data",JObject.FromObject(task).ToString())
                .Build();
        }
        

        private ITrigger CreateTriggerForJob(TaskMetadata jobMetadata, IJobDetail detailForJob)
        {
            return TriggerBuilder.Create()
            .WithIdentity(jobMetadata.TaskId.ToString())
            .WithCronSchedule(jobMetadata.CronExpression)
            .WithDescription(jobMetadata.TaskName)
            .ForJob(detailForJob)
            .Build();
        }


        

        private ITrigger CreateSimpleTrigger(SimpleTask task)
        {
            ITrigger simpleTrigger = null;
            switch (task.ScheduleRecurrence)
            {
                case RecurrenceType.Minutes:
                    simpleTrigger = TriggerBuilder.Create()
                        .WithIdentity(task.Id.ToString())
                        .WithDescription(task.TaskName)
                        .WithSimpleSchedule(s=> {
                            s.WithIntervalInMinutes(int.Parse(task.ScheduleData["every"].ToString()));
                            s.RepeatForever();
                        })
                        .Build();
                    break;
                case RecurrenceType.Hourly:

                    //JObject tempData = JObject.Parse(task.ScheduleData.ToString());

                    if (task.ScheduleData.ContainsKey("every"))
                    {
                        
                        simpleTrigger = TriggerBuilder.Create()
                            .WithIdentity(task.Id.ToString())
                            .WithDescription(task.TaskName)
                            .WithSimpleSchedule(s =>
                            {
                                s.WithIntervalInHours(int.Parse(task.ScheduleData["every"].ToString()));
                            })
                            .Build();
                    }
                    else
                    {
                        string startAtHourVal = task.ScheduleData["startat"].ToString();
                        DateTime tempNow = DateTime.Now;
                        DateTime finalStartAt = DateTime.Parse($"{tempNow.Day}-{tempNow.Month}-{tempNow.Year} {startAtHourVal}:00");

                        simpleTrigger = TriggerBuilder.Create()
                             .WithIdentity(task.Id.ToString())
                             .WithDescription(task.TaskName)
                             .StartAt(finalStartAt)
                             .Build();
                    }

                    break;
                case RecurrenceType.Daily:

                    simpleTrigger = TriggerBuilder.Create()
                        .WithIdentity(task.Id.ToString())
                        .WithDescription(task.TaskName)
                        .WithDailyTimeIntervalSchedule(s =>
                        {
                            s.WithIntervalInHours(24);
                            if (task.ScheduleData.ContainsKey("recurance"))
                            {
                                string[] startat = task.ScheduleData["startat"].ToString().Split(":");
                                if (task.ScheduleData["recurance"].ToString().ToLower() == "every")
                                {
                                    s.OnEveryDay();
                                    
                                    s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(int.Parse(startat[0]), int.Parse(startat[1])));
                                }
                                else
                                {
                                    s.OnMondayThroughFriday();
                                    
                                    s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(int.Parse(startat[0]), int.Parse(startat[1])));
                                }
                            }
                        })
                        .Build();
                    break;
                case RecurrenceType.Weekly:
                    break;
                case RecurrenceType.Monthly:
                    break;
                case RecurrenceType.Yearly:
                    break;
                default:
                    break;
            }

            return simpleTrigger;
        }

        private bool isValidScheduleData(SimpleTask task)
        {
            switch (task.ScheduleRecurrence)
            {
                case RecurrenceType.Minutes:
                    int minuteVal = int.Parse(task.ScheduleData["every"].ToString());

                    if ((minuteVal < 60) && (minuteVal > 0))
                        return true;
                    return false;

                case RecurrenceType.Hourly:

                    if (task.ScheduleData.ContainsKey("every"))
                    {
                        int hourVal = int.Parse(task.ScheduleData["every"].ToString());

                        if ((hourVal < 25) && (hourVal > 0))
                            return true;
                        return false;
                    }

                    string startAtHourVal = task.ScheduleData["startat"].ToString();

                    Console.WriteLine("VAL => " + startAtHourVal);
                    DateTime tempNow = DateTime.Now;
                    DateTime toEval = DateTime.Parse($"{tempNow.Day}-{tempNow.Month}-{tempNow.Year} {startAtHourVal}:00");
                    if ( toEval> DateTime.Now)
                    {
                        Console.WriteLine(toEval);
                        return true;
                    }
                    return false;

                case RecurrenceType.Daily:
                    return true;
                   
                //case RecurrenceType.Weekly:
                //    break;
                //case RecurrenceType.Monthly:
                //    break;
                //case RecurrenceType.Yearly:
                //    break;
                default:
                    return false;
            }
        }

        //Check if Jobkey Matched Job
        private async Task<bool> CheckIfKeyExists(string taskid)
        {
            Guid tempGuid;
            if (Guid.TryParse(taskid, out tempGuid))
            {
                var jKey = new JobKey(tempGuid.ToString());
                return await scheduler.CheckExists(jKey);
            }
            return false;
        }

        private async Task<bool> CheckIfKeyExists(List<string> taskids)
        {
            bool good = false;
            
            foreach( var id in taskids)
            {
                Guid tempGuid;
                if (Guid.TryParse(id, out tempGuid))
                {
                    var jKey = new JobKey(tempGuid.ToString());
                    good = await scheduler.CheckExists(jKey);
                    if (good == false) break;
                }
            }
            
            return good;
        }

        //GETS ALL JOBS
        private async Task<Array> GetScheduledTasks()
        {
            IList<string> TaskGroups = (IList<String>)await scheduler.GetJobGroupNames();
            List<ScheduledTasks> _scheduledTasks = new List<ScheduledTasks>();
            foreach (var group in TaskGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var TaskKeys = await scheduler.GetJobKeys(groupMatcher);
                foreach (var Taskkey in TaskKeys)
                {
                    var jdetail = await scheduler.GetJobDetail(Taskkey);
                    var triggers = await scheduler.GetTriggersOfJob(Taskkey);
                    foreach (var trigger in triggers)
                    {
                        string rowClass = string.Empty;
                        TriggerState state = await scheduler.GetTriggerState(trigger.Key);
                        if (state == TriggerState.Paused)
                            rowClass = "paused";

                        _scheduledTasks.Add(new ScheduledTasks
                        {
                            DT_RowId = trigger.Key.Name,
                            DT_RowClass = rowClass,
                            Group = group,
                            TaskName = trigger.Description,
                            Discription = jdetail.Description,
                            NextFireTime = trigger.GetNextFireTimeUtc().HasValue ? trigger.GetNextFireTimeUtc().Value.DateTime.ToLocalTime().ToString("HH:mm:ss dd-MMM-yyyy").ToUpper() : "N/A",
                            PreviousFireTime = trigger.GetPreviousFireTimeUtc().HasValue ? trigger.GetPreviousFireTimeUtc().Value.DateTime.ToLocalTime().ToString("HH:mm:ss dd-MMM-yyyy").ToUpper() : "N/A"
                        });


                    }
                }
            }
            return _scheduledTasks.ToArray();
        }

    }
}
