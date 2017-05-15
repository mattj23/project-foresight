using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using Foresight.Estimators;

namespace Foresight.Simulation
{
    /// <summary>
    /// This class holds data related to a single simulation of a single task, which 
    /// allows the original PertTask object to remain unchanged.
    /// </summary>
    public class SimulatedTaskData
    {
        private readonly IEstimator _estimator;
        private double _requiredTime;
        private double _spentTime;



        // A reference to the original task
        public PertTask Task { get; }

        public double RequiredTime => _requiredTime;
        public bool IsComplete => this._spentTime >= this._requiredTime;

        public SimulatedTaskData(PertTask task, IEstimator estimator)
        {
            this.Task = task;
            this._estimator = estimator;
            this.Reset();
        }

        /// <summary>
        /// Resets the SimulatedTaskData state for a new simulation, which picks a new time from the RNG for the 
        /// necessary time to complete the task, resets the spent time 
        /// </summary>
        public void Reset()
        {
            this._requiredTime = this._estimator.RandomValue(Task.TimeEstimate);
            this._spentTime = 0;

        }
    }
}