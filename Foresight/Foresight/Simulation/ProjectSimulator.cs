using System;
using System.Collections.Generic;
using System.Linq;
using Foresight.Estimators;
using Foresight.Tools;

namespace Foresight.Simulation
{
    /// <summary>
    /// Project simulator class
    /// 
    /// This class handles the time-based simulation of a project from start to finish.  Initialized with 
    /// a Project class to provide the framework for the program, it then begins a simulation at day 0 and 
    /// crawls forward each day, allocating resources for that day and making progress on tasks.  The simulation
    /// ends when all tasks are complete, and the statistics for the simulation are collected.
    /// </summary>
    public class ProjectSimulator
    {
        private Project _baseProject;


        private Dictionary<Guid, SimulatedTaskData> _taskDataById;
        private Dictionary<Guid, double> _nodePathTimes;

        private List<PertTask> _availableTasks;     // Tasks which are available to make progress on

        public ProjectSimulator(Project project)
        {
            this._baseProject = project;
            var estimator = new TriangularEstimator();

            // Generate the task simulation data 
            _availableTasks = new List<PertTask>();
            _taskDataById = new Dictionary<Guid, SimulatedTaskData>();
            foreach (var baseProjectTask in this._baseProject.Tasks)
            {
                _taskDataById.Add(baseProjectTask.Id, new SimulatedTaskData(baseProjectTask, estimator));
            }

            // Generate the most likely node path times
            _nodePathTimes = PathTools.NetworkModePathLengths(_baseProject.Tasks);

        }

        public SimulationResult Simulate()
        {
            var result = new SimulationResult(this._baseProject);

            // Prime the simulation by resetting all of the tasks and locating the tasks with no ancestors
            _availableTasks.Clear();
            foreach (var simulatedTaskData in _taskDataById)
            {
                simulatedTaskData.Value.Reset();
                if (!simulatedTaskData.Value.Task.Ancestors.Any())
                    _availableTasks.Add(simulatedTaskData.Value.Task);
            }

            // Go day by day and make progress until nothing is left  
            double masterClock = 0;
            while (_availableTasks.Any())
            {
                // Simulate a day's worth of progress

                // Find all tasks that don't require resources
                foreach (var task in _availableTasks.Where(x => !x.Resources.Any()).ToList())
                {
                    var taskData = _taskDataById[task.Id];

                    // Make progress on the task
                    taskData.AddTime(1.0, masterClock);
                    if (taskData.IsComplete)
                        this.SimulateTaskCompletion(task);
                }

                // Construct the resource availability for this day
                var resources = new Dictionary<string, double>();
                foreach (var employeeResource in this._baseProject.Organization.Employees)
                {
                    resources.Add(employeeResource.Name, 1.0);
                }
                //TODO: Add base resource groups

                // Current schedule prioritization: first, the most resources necessary
                // Second, the longest critical path
                // Find activities that require the most in the way of resources
                var resourceTasks = _availableTasks.Where(x => x.Resources.Any())
                    .OrderBy(x => x.Resources.Count)
                    .ThenBy(x => _nodePathTimes[x.Id])
                    .Reverse()
                    .ToList();

                foreach (var task in resourceTasks)
                {
                    var taskData = _taskDataById[task.Id];
                    // Check to find the least available amount of the unified resources
                    double availability = task.Resources.Select(x => resources[x.Name]).Min();

                    if (availability == 0)
                        continue;

                    // Add that amount and remove it from each resource
                    double spentTime = taskData.AddTime(availability, masterClock);
                    foreach (IResource resource in task.Resources)
                    {
                        resources[resource.Name] -= spentTime;
                        result.RecordResourceUsage(resource.Name, task.Id, masterClock, spentTime);
                    }

                    // Check the completion state 
                    if (taskData.IsComplete)
                        this.SimulateTaskCompletion(task);
                }

                masterClock += 1;
            }

            result.TotalCompletionDays = masterClock;
            return result;
        }


        /// <summary>
        /// Perform the necessary housekeeping when a task is completed, including removing it from the 
        /// available tasks and adding its descendants to the available list
        /// </summary>
        /// <param name="task"></param>
        private void SimulateTaskCompletion(PertTask task)
        {
            var taskData = _taskDataById[task.Id];
            if (!taskData.IsComplete)
                throw new ArgumentException($"Task '{task.Name}' was passed to completion method but was not actually complete");

            _availableTasks.Remove(task);
            foreach (var taskDescendant in task.Descendants)
            {
                if (AreTaskAncestorsDone(taskDescendant))
                    _availableTasks.Add(taskDescendant);
            }
        }

        /// <summary>
        /// Check to make sure that all of a task's ancestors are ready
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool AreTaskAncestorsDone(PertTask task)
        {
            foreach (var taskAncestor in task.Ancestors)
            {
                if (!_taskDataById[taskAncestor.Id].IsComplete)
                    return false;
            }
            return true;
        }
    }
}