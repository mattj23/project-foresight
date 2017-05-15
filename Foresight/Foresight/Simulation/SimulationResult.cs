using System;
using System.Collections.Generic;

namespace Foresight.Simulation
{
    public class SimulationResult
    {
        public double TotalCompletionDays { get; set; }

        public Dictionary<string, List<WorkTimeUsage>> ResourceUtilization { get; set; }

        public SimulationResult()
        {
            this.ResourceUtilization = new Dictionary<string, List<WorkTimeUsage>>();
        }

        public void RecordResourceUsage(string resourceName, Guid taskId, double simulationClock, double days)
        {
            var usage = WorkTimeUsage.Create(taskId, simulationClock, days);
            if (ResourceUtilization.ContainsKey(resourceName))
            {
                ResourceUtilization[resourceName].Add(usage);
            }
            else
            {
                ResourceUtilization.Add(resourceName, new List<WorkTimeUsage>{usage});
            }
        }
    }
}