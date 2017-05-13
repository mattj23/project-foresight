using System;
using System.Collections.Generic;
using System.Linq;

namespace Foresight.Serialization
{
    public class SerializeablePertTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Estimate TimeEstimate { get; set; }

        public List<string> Employees { get; set; }

        public List<Guid> Ancestors { get; set; }
        public List<Guid> Descendants { get; set; }

        public static SerializeablePertTask FromPertTask(PertTask item)
        {
            return new SerializeablePertTask
            {
                Name = item.Name,
                Id = item.Id,
                Description = item.Description,
                TimeEstimate = item.TimeEstimate,
                Employees = item.Employees.Select(x => x.Name).ToList(),
                Ancestors = item.Ancestors.Select(x => x.Id).ToList(),
                Descendants = item.Descendants.Select(x => x.Id).ToList()
            };

        }
        
    }
}