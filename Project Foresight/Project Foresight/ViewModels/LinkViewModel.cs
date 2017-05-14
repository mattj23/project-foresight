using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class LinkViewModel : INotifyPropertyChanged
    {
        private TaskViewModel _start;
        private TaskViewModel _end;
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskViewModel Start
        {
            get { return _start; }
            set
            {
                if (Equals(value, _start)) return;

                _start = value;

                OnPropertyChanged();
            }
        }

        public TaskViewModel End
        {
            get { return _end; }
            set
            {
                if (Equals(value, _end)) return;

                _end = value;

                OnPropertyChanged();
            }
        }

//        public LinkViewModel(TaskViewModel start, TaskViewModel end)
//        {
//            this.Start = start;
//            this.End = end;
//        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}