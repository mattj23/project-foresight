using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using Foresight;
using Foresight.Simulation;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MathNet.Numerics.Statistics;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class SimulationToolViewModel : INotifyPropertyChanged
    {
        private double _simulationTime;
        private int _iterationCount;
        private ProbabilityDensityData _selectedDensityChart;
        private bool _isGenerated;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SimulationComplete;

        public AppViewModel Parent { get; set; }

        public double SimulationTime
        {
            get { return _simulationTime; }
            set
            {
                if (value.Equals(_simulationTime)) return;
                _simulationTime = value;
                OnPropertyChanged();
            }
        }

        public int IterationCount
        {
            get { return _iterationCount; }
            set
            {
                if (value == _iterationCount) return;
                _iterationCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsGenerated
        {
            get { return _isGenerated; }
            set
            {
                if (value == _isGenerated) return;
                _isGenerated = value;
                OnPropertyChanged();
            }
        }

        // Multithreading and validation objects
        private bool _isValid;
        private Project _workingProject;

        // Chart Objects
        public ObservableCollection<ProbabilityDensityData> ProbabilityItems { get; set; }

        public ProbabilityDensityData SelectedDensityChart
        {
            get { return _selectedDensityChart; }
            set
            {
                if (Equals(value, _selectedDensityChart)) return;
                _selectedDensityChart = value;
                OnPropertyChanged();
            }
        }

        public SimulationToolViewModel(AppViewModel appViewModel)
        {
            this.Parent = appViewModel;
            this.IterationCount = 10000;
            this.ProbabilityItems = new ObservableCollection<ProbabilityDensityData>();
        }


        public void Invalidate()
        {
            _isValid = false;
        }

        public void RunMonteCarloSimulation()
        {
            // Update this with a local deep copy
            _isValid = true;
            _workingProject = this.Parent.Project.DeepCopy().Model;

            Stopwatch clock = new Stopwatch();
            clock.Start();
            
            // These double arrays store the double floating point result value for the simulation iteration at
            // the index value.  These distributions are then analyzed as probability models.
            double[] completionTimes = new double[this.IterationCount];
            double[] resourceCosts = new double[this.IterationCount];
            double[] fixedCosts = new double[this.IterationCount];

            // The resourceTime dictionary has the employee names as the key and the time usage distribution 
            // as the value.
            var resourceTime = _workingProject.Organization.Employees.ToDictionary(organizationEmployee => organizationEmployee.Name, organizationEmployee => new double[IterationCount]);

            // The taskStartTimes dictionary has the task ID as the key and the starting time distribution as the value
            var taskStartTimes = _workingProject.Tasks.ToDictionary(x => x.Id, x => new double[IterationCount]);

            // The taskEndTimes dictionary has the task ID as the key and the task ending time distribution as the value
            var taskEndTimes = _workingProject.Tasks.ToDictionary(x => x.Id, x => new double[IterationCount]);

            // The fixedCostsByCategory dictionary has the fixed cost category name as the key and the category total cost
            // distribution as the value
            var categories = new HashSet<string>(_workingProject.FixedCosts.Select(x => x.Category));
            var fixedCostsByCategory = categories.ToDictionary(category => category, category => new double[this.IterationCount]);

            Parallel.For(0, IterationCount, i =>
            {
                var simulator = new ProjectSimulator(_workingProject);
                var result = simulator.Simulate();

                completionTimes[i] = result.TotalCompletionDays;
                resourceCosts[i] = result.TotalResourceCost();

                // Aggregate the usage times for each resource, by name
                foreach (string resourceName in result.ResourceUtilization.Keys)
                    resourceTime[resourceName][i] = result.ResourceUtilization[resourceName].Select(x => x.Amount).Sum();

                // Aggregate the distribution of start and end times for each task
                foreach (var taskId in result.TaskStartTime.Keys)
                {
                    taskStartTimes[taskId][i] = result.TaskStartTime[taskId];
                    taskEndTimes[taskId][i] = result.TaskEndTime[taskId];
                }

                // Aggregate the fixed costs
                double totalFixedCost = 0;
                foreach (string category in categories)
                {
                    // Get the keys of the fixed costs in this category
                    var keys = _workingProject.FixedCosts.Where(x => x.Category == category).Select(x => x.Id).ToList();

                    // Get the total cost of these keys
                    double categoryCost = keys.Select(x => result.FixedCostValues[x]).Sum();
                    totalFixedCost += categoryCost;
                    fixedCostsByCategory[category][i] = categoryCost;
                }
                fixedCosts[i] = totalFixedCost;
            });

            var resultDistributions = new List<ProbabilityDensityData>();
            var taskSimulationData = new Dictionary<Guid, TaskSimulationData>();

            // Lambda to make sure that the result distribution is only added if not empty
            var addResultDistribution = new Action<ProbabilityDensityData>(x =>
            {
                if (!x.IsEmpty)
                    resultDistributions.Add(x);
            });

            // Main simulation results
            addResultDistribution(new ProbabilityDensityData(completionTimes, "Project Duration")
            {
                XLabel =  "Days",
                Category = "Main",
                ValueFormatter = x => $"{x:N0} days"
            });
            addResultDistribution(new ProbabilityDensityData(resourceCosts, "Resource Costs")
            {
                XLabel = "Dollars",
                Category = "Main",
                ValueFormatter = x => (x > 1000) ? $"${x / 1000:N0}k" : x.ToString("C0")
            });
            addResultDistribution(new ProbabilityDensityData(fixedCosts, "Fixed Project Costs")
            {
                XLabel = "Dollars",
                Category = "Main",
                ValueFormatter = x => (x > 1000) ? $"${x / 1000:N0}k" : x.ToString("C0")
            });
            
            // Resource Committment results
            foreach (var resourceTimeData in resourceTime)
            {
                addResultDistribution(new ProbabilityDensityData(resourceTimeData.Value, $"{resourceTimeData.Key}")
                {
                    Category = "Resource Commitment",
                    XLabel = "Days",
                    ValueFormatter = x => $"{x:N0} days"
                });
            }

            // Fixed cost category
            foreach (var fixedCost in fixedCostsByCategory)
            {
                addResultDistribution(new ProbabilityDensityData(fixedCost.Value, $"{fixedCost.Key}")
                {
                    XLabel = "Dollars",
                    Category = "Project Fixed Costs",
                    ValueFormatter = x => (x > 1000) ? $"${x / 1000:N0}k" : x.ToString("C0")
                });
            }

            // Task start and end times
            foreach (var taskStart in taskStartTimes)
            {
                var task = _workingProject.GetTaskById(taskStart.Key);
                var data = new ProbabilityDensityData(taskStart.Value, task.Name)
                {
                    Category = "Task Start Day",
                    XLabel = "Days",
                    ValueFormatter = x => $"Day {x:N0}"
                };
                addResultDistribution(data);

                taskSimulationData.Add(task.Id, new TaskSimulationData
                {
                    MedianStart = data.MedianValue,
                    HighStart = data.UpperConfidence,
                    LowStart = data.LowerConfidence
                });
            }

            foreach (var taskStart in taskEndTimes)
            {
                var task = _workingProject.GetTaskById(taskStart.Key);
                var data = new ProbabilityDensityData(taskStart.Value, task.Name)
                {
                    Category = "Task End Day",
                    XLabel = "Days",
                    ValueFormatter = x => $"Day {x:N0}"
                };
                addResultDistribution(data);
                taskSimulationData[task.Id].MedianEnd = data.MedianValue;
                taskSimulationData[task.Id].HighEnd = data.UpperConfidence;
                taskSimulationData[task.Id].LowEnd = data.LowerConfidence;
            }

            clock.Stop();

            // If we've been invalidated, we exit here
            if (!_isValid)
            {
                return;
            }

            // Set simulated data
            this.Parent.Project.IsSimulationDataValid = true;

            // Add the probability items
            this.ProbabilityItems.Clear();
            foreach (var probabilityDensityData in resultDistributions)
            {
                this.ProbabilityItems.Add(probabilityDensityData);
            }

            // Set the simulated task data
            foreach (var simData in taskSimulationData)
            {
                var task = this.Parent.Project.GetTaskById(simData.Key);
                task.SimulatedData = simData.Value;
            }

            this.SimulationTime = clock.Elapsed.TotalSeconds;
            this.SimulationComplete?.Invoke(this, EventArgs.Empty);
        }

       
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}