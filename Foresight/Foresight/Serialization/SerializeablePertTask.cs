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

        public List<Guid> Ancestors { get; set; }
        public List<Guid> Descendants { get; set; }

        public List<string> ResourceNames { get; set; }

        public static SerializeablePertTask FromPertTask(PertTask item)
        {
            return new SerializeablePertTask
            {
                Name = item.Name,
                Id = item.Id,
                Description = item.Description,
                TimeEstimate = item.TimeEstimate,
                Ancestors = item.Ancestors.Select(x => x.Id).ToList(),
                Descendants = item.Descendants.Select(x => x.Id).ToList(),
                ResourceNames = item.Resources.Select(x => x.Name).ToList(),
            };

        }

        /// <summary>
        /// Converts a SerializeablePertTask into a PertTask, but without the resource
        /// list or any linking
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static PertTask ToUnlinkedPertTask(SerializeablePertTask item)
        {
            var working = new PertTask
            {
                Name = item.Name,
                Id = item.Id,
                Description = item.Description,
                TimeEstimate = item.TimeEstimate,
                Resources = new HashSet<IResource>()
            };

            if (working.TimeEstimate == null)
                working.TimeEstimate = new Estimate();

            return working;
        }
        
    }
}