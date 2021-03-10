using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CalWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarApiController : ControllerBase
    {
        // GET: api/<CalendarApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CalendarApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value "+id;
        }

        // POST api/<CalendarApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CalendarApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CalendarApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
