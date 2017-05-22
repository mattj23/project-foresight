using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private static Dictionary<string, Color> _colorDictionary = new Dictionary<string, Color>();

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
                OnPropertyChanged(nameof(Background));
            }
        }

        [JsonIgnore]
        public Brush Background
        {
            get
            {
                if (_colorDictionary.ContainsKey(this.ColorName))
                    return new SolidColorBrush(_colorDictionary[this.ColorName]);

                var colorType = typeof(System.Windows.Media.Colors);
                var property = colorType.GetProperty(this.ColorName);
                if (property == null)
                    return new SolidColorBrush(Colors.White);

                var colorObject = (Color) property.GetValue(null);
                _colorDictionary.Add(this.ColorName, colorObject);

                return new SolidColorBrush(colorObject);
            }
        }

        


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}