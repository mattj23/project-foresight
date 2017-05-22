using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {

        private string _name;
        private string _colorName;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string ColorName
        {
            get { return _colorName; }
            set
            {
                if (value == _colorName) return;
                _colorName = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Brush Background
        {
            get
            {
                Color? val = Enum.Parse(typeof(Colors), this.ColorName) as Color?;
                if (val == null)
                    return new SolidColorBrush(Colors.White);
                return new SolidColorBrush(val.Value);
            }
        }

        


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}