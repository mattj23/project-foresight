using System;
using System.Collections.Generic;
using System.Linq;

namespace Foresight.Simulation
{
    public class SimulationResult
    {
        private Project _referenceProject;

        public double TotalCompletionDays { get; set; }

        public Dictionary<string, List<WorkTimeUsage>> ResourceUtilization { get; set; }

        public SimulationResult(Project baseProject)
        {
            this._referenceProject = baseProject;
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

        public double CostForResource(string resourceName)
        {
            double hourlyRate = _referenceProject.Organization.GetResourceByName(resourceName).Rate;
            double days = ResourceUtilization[resourceName].Select(x => x.Amount).Sum();
            return hourlyRate * days * 8;
        }

        public double TotalResourceCost()
        {
            double totalCost = 0;
            foreach (var resourceName in this.ResourceUtilization.Keys)
            {
                totalCost += CostForResource(resourceName);
            }
            return totalCost;
        }


    }
}