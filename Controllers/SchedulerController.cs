using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

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

        // GET: api/<CalendarApiController>
        [HttpGet]
        public async Task<Array> Get()
        {
            
            return await GetScheduledTasks();
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
        ///     PUT /Unschedule
        ///     
        ///     {
        ///         taskid:task_id_goes_here
        ///     }
        ///     
        ///
        /// </remarks>
        /// <returns>Status code and messages where necessary</returns>
        /// <param name="taskid"></param>
        /// <response code="200">Reruns 201 status and 'Task Schdeuled'</response>
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
        /// Reschedule a task
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Reschedule
        ///     
        ///     {
        ///         taskid:task_id_goes_here
        ///     }
        ///     
        ///
        /// </remarks>
        /// <returns>Status code and messages where necessary</returns>
        /// <param name="taskid"></param>
        /// <response code="200">Reruns 201 status and 'Task Schdeuled'</response>
        /// <response code="400">If the json is not stuctured well or invalid data</response>  
        [HttpPut]
        [ApiVersion("1.0")]
        [Route("Reschedule")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RescheduleTask([FromBody]TaskIdFromBody taskid)
        {
            Guid tempGuid;
            if (Guid.TryParse(taskid.TaskID, out tempGuid))
            {
                //if (await CheckIfKeyExists(taskid))
                //    return BadRequest("Invalid Task ID");

                TriggerKey tkey = new TriggerKey(taskid.TaskID);
                JobKey jKey = new JobKey(taskid.TaskID);
                var jDetail = await scheduler.GetJobDetail(jKey);
                ScheduleTask task = JsonConvert.DeserializeObject<ScheduleTask>(jDetail.JobDataMap.GetString("data"));

                Console.WriteLine("Rescheduled Job Data");
                Console.WriteLine(JObject.FromObject(task).ToString());

                Type newTriggerTaskType = jDetail.JobType;

                
                var newTaskMetadata = new TaskMetadata(Guid.Parse(taskid.TaskID), newTriggerTaskType, task.TaskName, task.CronExpression);

                //scheduler.ScheduleJob(CreateTriggerForJob(newTaskMetadata))

                var isRescheduled = await scheduler.ScheduleJob(CreateTriggerForJob(newTaskMetadata,jDetail));

                string resp = "Is Null";
                if (isRescheduled == null)
                {
                    resp += " Trigger";
                }
                else
                {
                    resp = $"Trigger Created Dateoffset{isRescheduled}";
                }
                return Created(Request.Path, $"Task Rescheduled {resp}");
            }
            return BadRequest("Invalid Task Id");
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
        /// Delete a Single Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        [ApiVersion("1.0")]
        public async Task<IActionResult> DeleteTask([FromBody]TaskIdFromBody taskId)
        {
            try
            {
                var jKey = new JobKey(taskId.TaskID);
                if (await scheduler.CheckExists(jKey)) await scheduler.DeleteJob(jKey);

            }
            catch (Exception)
            {

                throw;
            }
            return Ok();
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
            .StoreDurably(true)
            .Build();
        }

        private ITrigger CreateTriggerForJob(TaskMetadata jobMetadata, IJobDetail detailForJob)
        {
            return TriggerBuilder.Create()
            .WithIdentity(jobMetadata.TaskId.ToString())
            .WithCronSchedule(jobMetadata.CronExpression)
            .WithDescription($"{jobMetadata.TaskName}")
            .ForJob(detailForJob)
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
                    var detail = await scheduler.GetJobDetail(Taskkey);
                    var triggers = await scheduler.GetTriggersOfJob(Taskkey);
                    foreach (var trigger in triggers)
                    {
                        _scheduledTasks.Add(new ScheduledTasks
                        {
                            DT_RowId = trigger.Key.Name,
                            Group = group,
                            TaskName = detail.Description,
                            NextFireTime = trigger.GetNextFireTimeUtc(),
                            PreviousFireTime = trigger.GetPreviousFireTimeUtc()
                        });
                    }
                }
            }
            return _scheduledTasks.ToArray();
        }

    }
}
