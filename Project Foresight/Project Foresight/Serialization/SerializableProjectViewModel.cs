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
    public class SerializableProjectViewModel
    {
        public static SerializableProjectViewModel FromProjectViewModel(ProjectViewModel viewModel)
        {
            var working = new SerializableProjectViewModel
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Organization = SerializableOrganizationViewModel.FromOrganizationViewModel(viewModel.Organization),
                FixedCosts = viewModel.FixedCosts.Select(x => x.Model).ToList(),
                
            };

            working.Tasks = viewModel.Tasks.Select(x => SerializablePertTask.FromPertTask(x.Model)).ToList();
            working.TaskPositions = new Dictionary<Guid, Point>();
            foreach (var viewModelTask in viewModel.Tasks)
            {
                working.TaskPositions.Add(viewModelTask.Id, viewModelTask.CenterPoint);
            }

            return working;
        }

        public static ProjectViewModel ToProjectViewModel(SerializableProjectViewModel working)
        {
            if (working.Organization == null)
            {
                working.Organization = new SerializableOrganizationViewModel();
            }
            var project = new ProjectViewModel
            {
                Name = working.Name,
                Description = working.Description,
                Organization = SerializableOrganizationViewModel.ToOrganizationViewModel(working.Organization)
            };
            project.Model.Organization = project.Organization.Model;

            if (working.FixedCosts == null)
                working.FixedCosts = new List<FixedCost>();
            foreach (var cost in working.FixedCosts)
            {
                project.FixedCosts.Add(new FixedCostViewModel(cost));
            }

            foreach (SerializablePertTask task in working.Tasks)
            {
                var taskViewmodel = new TaskViewModel(SerializablePertTask.ToUnlinkedPertTask(task));
                var category = project.Organization.Categories.FirstOrDefault(x => x.Name == task.Category);
                if (category == null)
                    taskViewmodel.Category = ProjectViewModel.EmptyCategory;
                else
                    taskViewmodel.Category = category;
                project.AddTask(taskViewmodel);
            }

            // Now go through each task and make the necessary links and linked attributes (like employees, categories, etc)
            foreach (SerializablePertTask serializeablePertTask in working.Tasks)
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
        public SerializableOrganizationViewModel Organization { get; set; }

        public List<SerializablePertTask> Tasks { get; set; }
        public Dictionary<Guid, Point> TaskPositions { get; set; }

        public List<FixedCost> FixedCosts { get; set; }
    }
}