using System;
using System.Collections.Generic;
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
        public bool IsComplete => this.RemainingTime <= 0;
        public double RemainingTime => this._requiredTime - this._spentTime;

        /// <summary>
        /// Item1 is the day the work was done, Item2 is the decimal day amount of work logged on the task
        /// </summary>
        public List<Tuple<double, double>> WorkLog => _workLog;

        private List<Tuple<double, double>> _workLog;

        public SimulatedTaskData(PertTask task, IEstimator estimator)
        {
            _workLog = new List<Tuple<double, double>>();
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
            this._workLog.Clear();
        }

        /// <summary>
        /// Adds up to the given amount of time to the task, increasing the spent time up to but not exceeding
        /// the total amount of required time for the task. Logs the time spent.
        /// </summary>
        /// <param name="days">Max amount of time (in days) to progress on the task</param>
        /// <param name="simulationClock">The simulation clock time</param>
        /// <returns>the amount of time actually consumed</returns>
        public double AddTime(double days, double simulationClock)
        {
            if (days >= this.RemainingTime)
            {
                double remaining = this.RemainingTime;
                _spentTime = _requiredTime;

                this._workLog.Add(Tuple.Create(simulationClock, remaining));
                return remaining;
            }

            _spentTime += days;
            this._workLog.Add(Tuple.Create(simulationClock, days));
            return days;
        }
    }
}