using System;
using System.Collections.Generic;
using System.Linq;

namespace Foresight.Serialization
{
    public class SerializablePertTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Estimate TimeEstimate { get; set; }

        public List<Guid> Ancestors { get; set; }
        public List<Guid> Descendants { get; set; }

        public List<string> ResourceNames { get; set; }

        public string Category { get; set; }

        public static SerializablePertTask FromPertTask(PertTask item)
        {
            return new SerializablePertTask
            {
                Name = item.Name,
                Id = item.Id,
                Description = item.Description,
                TimeEstimate = item.TimeEstimate,
                Ancestors = item.Ancestors.Select(x => x.Id).ToList(),
                Descendants = item.Descendants.Select(x => x.Id).ToList(),
                ResourceNames = item.Resources.Select(x => x.Name).ToList(),
                Category = item.Category
            };

        }

        /// <summary>
        /// Converts a SerializeablePertTask into a PertTask, but without the resource
        /// list or any linking
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static PertTask ToUnlinkedPertTask(SerializablePertTask item)
        {
            var working = new PertTask
            {
                Name = item.Name,
                Id = item.Id,
                Description = item.Description,
                TimeEstimate = item.TimeEstimate,
                Resources = new HashSet<IResource>(),
                Category =  item.Category
            };

            if (working.TimeEstimate == null)
                working.TimeEstimate = new Estimate();

            return working;
        }
        
    }
}