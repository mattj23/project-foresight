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

            var simulator = new ProjectSimulator(this.Parent.Project.Model);
            for (int i = 0; i < this.IterationCount; i++)
            {
                var result = simulator.Simulate();
                completionTimes[i] = result.TotalCompletionDays;
                resourceCosts[i] = result.TotalResourceCost();
            }

            ProbabilityItems.Clear();
            ProbabilityItems.Add(new ProbabilityDensityData(completionTimes, "Project Duration") {XLabel =  "Days"});
            ProbabilityItems.Add(new ProbabilityDensityData(resourceCosts, "Resource Costs") {XLabel =  "Dollars", ValueFormatter = x => x.ToString("c0")});
            this.SelectedDensityChart = ProbabilityItems[0];

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