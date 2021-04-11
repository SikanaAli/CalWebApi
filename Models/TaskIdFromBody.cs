using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class TaskIdFromBody
    {
        public List<string> TaskIDs { get; set; }
        public int Count { get; set; }
    }
}
