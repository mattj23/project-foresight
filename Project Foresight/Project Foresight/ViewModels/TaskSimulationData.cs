using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class TaskSimulationData : INotifyPropertyChanged
    {
        private double _highStart;
        private double _lowStart;
        private double _medianStart;
        private double _highEnd;
        private double _lowEnd;
        private double _medianEnd;
        public event PropertyChangedEventHandler PropertyChanged;

        public double MedianStart
        {
            get { return _medianStart; }
            set
            {
                if (value.Equals(_medianStart)) return;
                _medianStart = value;
                OnPropertyChanged();
            }
        }

        public double LowStart
        {
            get { return _lowStart; }
            set
            {
                if (value.Equals(_lowStart)) return;
                _lowStart = value;
                OnPropertyChanged();
            }
        }

        public double HighStart
        {
            get { return _highStart; }
            set
            {
                if (value.Equals(_highStart)) return;
                _highStart = value;
                OnPropertyChanged();
            }
        }

        public double MedianEnd
        {
            get { return _medianEnd; }
            set
            {
                if (value.Equals(_medianEnd)) return;
                _medianEnd = value;
                OnPropertyChanged();
            }
        }

        public double LowEnd
        {
            get { return _lowEnd; }
            set
            {
                if (value.Equals(_lowEnd)) return;
                _lowEnd = value;
                OnPropertyChanged();
            }
        }

        public double HighEnd
        {
            get { return _highEnd; }
            set
            {
                if (value.Equals(_highEnd)) return;
                _highEnd = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}