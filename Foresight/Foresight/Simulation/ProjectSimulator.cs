using System;
using System.Collections.Generic;
using System.Linq;
using Foresight.Estimators;

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

            

        }

        public SimulationResult Simulate()
        {
            var result = new SimulationResult();

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
                // Progress!

            }

            
            
            return result;
        }
    }
}