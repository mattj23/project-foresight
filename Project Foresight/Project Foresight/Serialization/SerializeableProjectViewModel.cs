using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Foresight;
using Foresight.Serialization;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Serialization
{
    public class SerializeableProjectViewModel
    {
        public static SerializeableProjectViewModel FromProjectViewModel(ProjectViewModel viewModel)
        {
            var working = new SerializeableProjectViewModel
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Organization = SerializableOrganization.FromOrganization(viewModel.Organization.Model)
            };

            working.Tasks = viewModel.Tasks.Select(x => SerializeablePertTask.FromPertTask(x.Model)).ToList();
            working.TaskPositions = new Dictionary<Guid, Point>();
            foreach (var viewModelTask in viewModel.Tasks)
            {
                working.TaskPositions.Add(viewModelTask.Id, viewModelTask.CenterPoint);
            }

            return working;
        }

        public static ProjectViewModel ToProjectViewModel(SerializeableProjectViewModel working)
        {
            if (working.Organization == null)
            {
                working.Organization = new SerializableOrganization();
            }
            var project = new ProjectViewModel
            {
                Name = working.Name,
                Description = working.Description,
                Organization = new OrganizationViewModel(SerializableOrganization.ToOrganization(working.Organization))
            };
            project.Model.Organization = project.Organization.Model;

            foreach (SerializeablePertTask task in working.Tasks)
            {
                project.AddTask(new TaskViewModel(SerializeablePertTask.ToUnlinkedPertTask(task)));
            }

            // Now go through each task and make the necessary links and linked attributes (like employees)
            foreach (SerializeablePertTask serializeablePertTask in working.Tasks)
            {
                var actualTask = project.GetTaskById(serializeablePertTask.Id);

                // The position in the diagram
                actualTask.X = working.TaskPositions[actualTask.Id].X;
                actualTask.Y = working.TaskPositions[actualTask.Id].Y;

                // Create the project network links
                foreach (Guid descendantId in serializeablePertTask.Descendants)
                {
                    var descendantTask = project.GetTaskById(descendantId);
                    project.AddLink(actualTask, descendantTask);
                }

                // Work out the resources
                foreach (var resourceName in serializeablePertTask.ResourceNames)
                {
                    var locatedResource = project.Organization.FindResourceByName(resourceName);
                    if (locatedResource != null)
                        actualTask.Resources.Add(locatedResource);
                }
            }


            return project;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public SerializableOrganization Organization { get; set; }

        public List<SerializeablePertTask> Tasks { get; set; }
        public Dictionary<Guid, Point> TaskPositions { get; set; }
    }
}