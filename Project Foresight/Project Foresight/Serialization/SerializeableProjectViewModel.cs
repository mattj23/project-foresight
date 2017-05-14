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
                Organization = viewModel.Organization.Model
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
                working.Organization = new Organization();
            }
            var project = new ProjectViewModel
            {
                Name = working.Name,
                Description = working.Description,
                Organization = new OrganizationViewModel(working.Organization)
            };

            foreach (SerializeablePertTask task in working.Tasks)
            {
                project.AddTask(new TaskViewModel(SerializeablePertTask.ToUnlinkedPertTask(task)));
            }

            // Now go through each task and make the necessary links and linked attributes (like employees)
            foreach (SerializeablePertTask serializeablePertTask in working.Tasks)
            {
                var actualTask = project.GetTaskById(serializeablePertTask.Id);
                actualTask.X = working.TaskPositions[actualTask.Id].X;
                actualTask.Y = working.TaskPositions[actualTask.Id].Y;
                foreach (Guid descendantId in serializeablePertTask.Descendants)
                {
                    var descendantTask = project.GetTaskById(descendantId);
                    project.AddLink(actualTask, descendantTask);
                }
            }


            return project;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public Organization Organization { get; set; }

        public List<SerializeablePertTask> Tasks { get; set; }
        public Dictionary<Guid, Point> TaskPositions { get; set; }
    }
}