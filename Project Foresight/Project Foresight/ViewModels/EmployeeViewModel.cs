using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged, IResource
    {
        private ResourceGroupViewModel _group;
        public event PropertyChangedEventHandler PropertyChanged;

        public Employee Model { get; }

        public ObservableCollection<ResourceGroupViewModel> ResourceGroups { get; }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged();
            }
        }

        public ResourceGroupViewModel Group
        {
            get => _group;
            set
            {
                if (Equals(value, _group)) return;
                _group = value;
                this.Model.Group = this.Group?.Model;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Rate));
            }
        }


        public double Rate => this.Model.Rate;

        public int Available => this.Model.Available;

        public EmployeeViewModel(ObservableCollection<ResourceGroupViewModel> resourceGroups)
        {
            ResourceGroups = resourceGroups;
            this.Model = new Employee();
            this.Group = new ResourceGroupViewModel();
        }

        public EmployeeViewModel(Employee model, ObservableCollection<ResourceGroupViewModel> resourceGroups)
        {
            this.Model = model;
            ResourceGroups = resourceGroups;
            this.Group = new ResourceGroupViewModel(model.Group);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}