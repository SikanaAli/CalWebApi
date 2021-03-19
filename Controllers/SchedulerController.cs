using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

using Quartz;
using Microsoft.AspNetCore.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.AspNetCore.Filters;
using CalWebApi.Scheduler;
using Microsoft.Extensions.Logging;
using Quartz.Impl.Matchers;
using CalWebApi.Helpers;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<CalendarApiController>
        [HttpGet]
        public async Task<List<ScheduledTasks>> Get()
        {
            var tasks = await GetScheduledTasks();
            return tasks;
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
        public async Task<IActionResult> ScheduleTask([FromBody]ScheduleTask task)
        {
            if (ModelState.IsValid)
            {
                if (CronExpression.IsValidExpression(task.CronExpression))
                {
                    if (scheduler.IsStarted)
                    {
                        //await scheduler.Start();
                        
                        var metadata = new TaskMetadata(Guid.NewGuid(), typeof(TaskJob),task.TaskName, task.CronExpression);
                        var Job = CreateJob(metadata,task);
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
        ///     PUT /Unschedule/{taskid}
        ///     
        ///     {
        ///         taskid:task_id_goes_here
        ///     }
        ///     
        ///
        /// </remarks>
        /// <returns>Status code and messages where necessary</returns>
        /// <param name="taskid"></param>
        /// <response code="201">Reruns 201 status and 'Task Schdeuled'</response>
        /// <response code="400">If the json is not stuctured well or invalid data</response>  
        [HttpPut]
        [ApiVersion("1.0")]
        [Route("Unschedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(JObject), typeof(ScheduleTaskExample))]
        public async Task<IActionResult> UnScheduleTask([FromBody]TaskIdFromBody taskid)
        {
            Guid id;
            if (Guid.TryParse(taskid.TaskID,out id))
            {
                TriggerKey key = new TriggerKey(taskid.TaskID);
                var isOk = await scheduler.UnscheduleJob(key);
                string resp = isOk ? "Task UnScheduled" : "Task Not Found";
                return Ok(resp);
            }
            return BadRequest("Invalid ID");
        }


        /// <summary>
        /// Reschedule a task that is in an Unscheduled
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns>Status code and messages where necessary</returns>
        [HttpPut]
        [ApiVersion("1.0")]
        [Route("Reschedule")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RescheduleTask(string taskid)
        {
            Guid tempGuid;
            if (Guid.TryParse(taskid, out tempGuid))
            {
                //if (await CheckIfKeyExists(taskid))
                //    return BadRequest("Invalid Task ID");

                TriggerKey tkey = new TriggerKey(taskid);
                JobKey jKey = new JobKey(taskid);
                var jDetail = await scheduler.GetJobDetail(jKey);
                ScheduleTask task = JsonConvert.DeserializeObject<ScheduleTask>(jDetail.JobDataMap.GetString("data"));

                Type newTriggerTaskType = jDetail.JobType;


                var newTaskMetadata = new TaskMetadata(Guid.Parse(taskid), newTriggerTaskType, task.TaskName, task.CronExpression);
                var isRescheduled = await scheduler.RescheduleJob(tkey, CreateTrigger(newTaskMetadata));
                return Created(Request.Path, "Task Rescheduled");
            }
            return BadRequest("Invalid Task Id");
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
        private IJobDetail CreateJob(TaskMetadata jobMetadata, ScheduleTask task)
        {
            return JobBuilder
            .Create(jobMetadata.TaskType)
            .UsingJobData("data",JObject.FromObject(task).ToString())
            .WithIdentity(jobMetadata.TaskId.ToString())
            .WithDescription($"{jobMetadata.TaskName}")
            .StoreDurably()
            .Build();
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


        //GETS ALL JOBS
        private async Task<List<ScheduledTasks>> GetScheduledTasks()
        {
            IList<string> TaskGroups = (IList<String>)await scheduler.GetJobGroupNames();
            List<ScheduledTasks> _scheduledTasks = new List<ScheduledTasks>();
            foreach (var group in TaskGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var TaskKeys = await scheduler.GetJobKeys(groupMatcher);
                foreach (var Taskkey in TaskKeys)
                {
                    var detail = await scheduler.GetJobDetail(Taskkey);
                    var triggers = await scheduler.GetTriggersOfJob(Taskkey);
                    foreach (var trigger in triggers)
                    {
                        _scheduledTasks.Add(new ScheduledTasks
                        {
                            Group = group,
                            TaskKey = Taskkey.Name,
                            TaskName = detail.Description,
                            TriggerKey = trigger.Key.Name,
                            NextFireTime = trigger.GetNextFireTimeUtc(),
                            PreviousFireTime = trigger.GetPreviousFireTimeUtc()
                        });
                    }
                }
            }
            return _scheduledTasks;
        }

    }
}
