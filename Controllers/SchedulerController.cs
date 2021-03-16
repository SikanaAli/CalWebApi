using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;

using Quartz;
using Microsoft.AspNetCore.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.AspNetCore.Filters;


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
        //Contructor
        public SchedulerController(IScheduler _scheduler)
        {
            scheduler = _scheduler;
        }

        // GET: api/<CalendarApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
        public IActionResult ScheduleTask([FromBody]ScheduleTask task)
        {
            if (ModelState.IsValid)
            {
                var ok = new JObject();
                
                return Created(Request.Path,"Task Scheduled");
            }
            else
            {

                var error = new JObject();
                error.Add("StatusCode", (int)HttpStatusCode.BadRequest);
                error.Add("error", JObject.FromObject(ModelState).ToString());

                return BadRequest(error.ToString());
            }
        }
    }
}
