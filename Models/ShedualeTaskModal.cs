﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.CustomAttributes;
namespace CalWebApi.Models
{
    public class ShedualeTaskModal
    {
        [SwaggerExclude]
        public int Id { get; set; }

        [nullable]
        public string TaskName { get; set; }

        public DateTime DateCreated { get; set; }

        public string CronExpression { get; set; }

        public string Discription { get; set; }
    }
}