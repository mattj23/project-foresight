using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Foresight.Simulation;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class SimulationToolViewModel : INotifyPropertyChanged
    {
        private double _simulationTime;
        private int _iterationCount;
        private double _meanResourceCost;
        private double _meanCompletionTime;
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

        public double MeanCompletionTime
        {
            get { return _meanCompletionTime; }
            set
            {
                if (value.Equals(_meanCompletionTime)) return;
                _meanCompletionTime = value;
                OnPropertyChanged();
            }
        }

        public double MeanResourceCost
        {
            get { return _meanResourceCost; }
            set
            {
                if (value.Equals(_meanResourceCost)) return;
                _meanResourceCost = value;
                OnPropertyChanged();
            }
        }


        public SimulationToolViewModel(AppViewModel appViewModel)
        {
            this.Parent = appViewModel;
            this.IterationCount = 10000;
        }

        public void RunMonteCarloSimulation()
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();



            var simulator = new ProjectSimulator(this.Parent.Project.Model);
            var result = simulator.Simulate();
            this.MeanCompletionTime = result.TotalCompletionDays;
            this.MeanResourceCost = result.TotalResourceCost();

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