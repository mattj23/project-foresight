using System;

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


        public ProjectSimulator(Project project)
        {
            this._baseProject = project;
        }

        public SimulationResult Simulate()
        {
            
            throw new NotImplementedException();
        }
    }
}