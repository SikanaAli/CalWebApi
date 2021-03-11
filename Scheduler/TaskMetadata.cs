using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalWebApi.Scheduler
{
    public class TaskMetadata
    {
        public Guid TaskId { get; set; }
        public Type TaskType { get; }
        public string TaskName { get; }
        public string CronExpression { get; }
        public TaskMetadata(Guid Id, Type taskType, string taskName,string cronExpression)
        {
            TaskId = Id;
            TaskType = taskType;
            TaskName = taskName;
            CronExpression = cronExpression;
        }
    }
}
