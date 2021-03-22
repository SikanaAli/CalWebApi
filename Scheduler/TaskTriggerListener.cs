using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalWebApi.Scheduler
{
    public class TaskTriggerListener : ITriggerListener
    {
        public string Name => "Scheduler Trigger Listener";

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine($"Trigger for {trigger.Key.Name} Complete");
        }

        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine($"Trigger Fired : {trigger.Key.Name}"); 
        }

        public async Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine($"Trigger Misfired : {trigger.Key.Name}");
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return false;
        }
    }
}
