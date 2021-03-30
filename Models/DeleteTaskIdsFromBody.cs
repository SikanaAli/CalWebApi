using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class DeleteTaskIdsFromBody
    {
        public List<string> TaskIds { get; set; }
        public int Count { get; set; }
    }
}
