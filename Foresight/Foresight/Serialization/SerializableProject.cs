using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Foresight.Serialization
{
    public class SerializableProject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<SerializeablePertTask> Tasks { get; set; }

        public static SerializableProject FromProject(Project item)
        {
            return new SerializableProject
            {
                Name = item.Name,
                Description = item.Description,
                Tasks = item.Tasks.Select(SerializeablePertTask.FromPertTask).ToList(),
            };
        }

        public static string SerializeProject(Project item)
        {
            return JsonConvert.SerializeObject(FromProject(item));
        }
    }
}