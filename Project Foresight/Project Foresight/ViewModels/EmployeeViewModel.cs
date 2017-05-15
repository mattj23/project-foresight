using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged, IResource
    {
        private ResourceGroupViewModel _group;
        private string _resourceGroupName;
        public event PropertyChangedEventHandler PropertyChanged;

        public Employee Model { get; }


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
            get { return _group; }
            set
            {
                if (Equals(value, _group)) return;
                _group = value;
                this.Model.Group = this.Group?.Model;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Rate));
            }
        }

        public string ResourceGroupName
        {
            get { return _resourceGroupName; }
            set
            {
                if (value == _resourceGroupName) return;
                _resourceGroupName = value;
                OnPropertyChanged();
            }
        }

        public double Rate => this.Model.Rate;

        public int Available => this.Model.Available;

        public EmployeeViewModel()
        {
            this.Model = new Employee();
            this.Group = new ResourceGroupViewModel();
        }

        public EmployeeViewModel(Employee model)
        {
            this.Model = model;
            this.Group = new ResourceGroupViewModel(model.Group);
            this.ResourceGroupName = this.Group.Name;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}