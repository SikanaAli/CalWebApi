using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CalWebApi.Controllers
{
    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CalendarApiController : ControllerBase
    {
        // GET: api/<CalendarApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        

        /// <summary>
        /// Schedule a new task
        /// </summary>
        /// <param name="task"></param>
        
        [HttpPost]
        public void ScheduleTask([FromBody]ShedualeTaskModal task)
        {

        }
    }
}
