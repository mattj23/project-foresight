using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        private string _message;
        private SolidColorBrush _color;
        private bool _isRemoving;

        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message) return;
                _message = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Color
        {
            get { return _color; }
            set
            {
                if (Equals(value, _color)) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        public bool IsRemoving
        {
            get { return _isRemoving; }
            set
            {
                if (value == _isRemoving) return;
                _isRemoving = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}