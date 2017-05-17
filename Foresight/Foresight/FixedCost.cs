using System;

namespace Foresight
{
    public class FixedCost
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public Estimate CostEstimate { get; set; }

        public Guid Id { get; set; }

        public FixedCost()
        {
            this.Id = Guid.NewGuid();
        }
    }
}