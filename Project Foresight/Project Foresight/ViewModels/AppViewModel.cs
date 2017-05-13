using System.ComponentModel;
using System.Runtime.CompilerServices;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static AppViewModel _global;
        public static AppViewModel Global
        {
            get
            {
                if (_global == null)
                    _global = new AppViewModel();
                return _global;
            }
        }

        public ProjectViewModel Project { get; set; }

        public AppViewModel()
        {
            this.Project = new ProjectViewModel {Name = "Hello world!"};
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}