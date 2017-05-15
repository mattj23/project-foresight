using System;

namespace Foresight.Simulation
{
    public class WorkTimeUsage
    {
        public double DayNumber { get; set; }
        public double Amount { get; set; }

        public Guid TaskId { get; set; }

        public static WorkTimeUsage Create(Guid taskId, double dayNumber, double amount)
        {
            return new WorkTimeUsage
            {
                TaskId = taskId,
                Amount = amount,
                DayNumber = dayNumber
            };
        }
    }
}