using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Models
{
    public class ShedualeTaskModal
    {
        public int Id { get; set; }
        public string TaskName { get; set; }

        public DateTime DateCreated { get; set; }

        public string MyProperty { get; set; }
        public string CronExpression { get; set; }
    }
}
