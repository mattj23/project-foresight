using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public event PropertyChangedEventHandler PropertyChanged;

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

        public void RunMonteCarloSimulation()
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            
            double[] completionTimes = new double[this.IterationCount];
            double[] resourceCosts = new double[this.IterationCount];

            var resourceTime = new Dictionary<string, double[]>();
            

            var simulator = new ProjectSimulator(this.Parent.Project.Model);
            for (int i = 0; i < this.IterationCount; i++)
            {
                var result = simulator.Simulate();
                completionTimes[i] = result.TotalCompletionDays;
                resourceCosts[i] = result.TotalResourceCost();

                foreach (string resourceName in result.ResourceUtilization.Keys)
                {
                    if (!resourceTime.ContainsKey(resourceName))
                        resourceTime.Add(resourceName, new double[this.IterationCount]);
                    resourceTime[resourceName][i] = result.ResourceUtilization[resourceName].Select(x => x.Amount).Sum();
                }
            }

            ProbabilityItems.Clear();

            // Main simulation results
            ProbabilityItems.Add(new ProbabilityDensityData(completionTimes, "Project Duration")
            {
                XLabel =  "Days",
                Category = "Main",
                ValueFormatter = x => $"{x:N0} days"
            });
            ProbabilityItems.Add(new ProbabilityDensityData(resourceCosts, "Resource Costs")
            {
                XLabel =  "Dollars",
                Category = "Main",
                ValueFormatter = x => (x > 1000) ? $"${x/1000:N0}k" : x.ToString("C0")
            });
            this.SelectedDensityChart = ProbabilityItems[0];

            // Resource Committment results
            foreach (var resourceTimeData in resourceTime)
            {
                ProbabilityItems.Add(new ProbabilityDensityData(resourceTimeData.Value, $"{resourceTimeData.Key} (time)")
                {
                    Category = "Resource Commitment",
                    XLabel = "Days",
                    ValueFormatter = x => $"{x:N0} days"
                });
            }


            clock.Stop();
            this.SimulationTime = clock.Elapsed.TotalSeconds;
        }

       
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}