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

            var t1 = new TaskViewModel {X = 0, Y = 0, Name = "Test Task 1!"};
            var t2 = new TaskViewModel {X = 200, Y = 0, Name = "Test Task 2!"};
            this.Project.AddTask(t1);
            this.Project.AddTask(t2);
            
            t1.LinkToDescendant(t2);
            this.Project.Links.Add(new LinkViewModel {Start = this.Project.Tasks[0], End = this.Project.Tasks[1]});

        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}