using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Foresight.Serialization
{
    public class SerializableProject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public SerializableOrganization Organization { get; set; }

        public List<SerializeablePertTask> Tasks { get; set; }

        public static SerializableProject FromProject(Project item)
        {
            return new SerializableProject
            {
                Name = item.Name,
                Description = item.Description,
                Tasks = item.Tasks.Select(SerializeablePertTask.FromPertTask).ToList(),
                Organization = SerializableOrganization.FromOrganization(item.Organization)
            };
        }

        public static Project ToProject(SerializableProject item)
        {
            var working = new Project
            {
                Name = item.Name,
                Description = item.Description,
                Organization = SerializableOrganization.ToOrganization(item.Organization)
            };

            // Add all the tasks to the project tasks, but without linking
            foreach (var serTask in item.Tasks)
                working.AddTask(SerializeablePertTask.ToUnlinkedPertTask(serTask));

            // Now go through and link and add employees
            foreach (var serTask in item.Tasks)
            {
                var actualTask = working.GetTaskById(serTask.Id);
                foreach (Guid descendantId in serTask.Descendants)
                {
                    var descendant = working.GetTaskById(descendantId);
                    actualTask.LinkToDescendant(descendant);
                }
            }


            return working;
        }

        public static string Serialize(Project item)
        {
            return JsonConvert.SerializeObject(FromProject(item), Formatting.Indented);
        }

        public static Project Deserialize(string data)
        {
            var working = JsonConvert.DeserializeObject<SerializableProject>(data);
            return ToProject(working);
        }
    }
}