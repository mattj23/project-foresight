using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged, IResource
    {
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

        public ResourceGroup Group
        {
            get { return this.Model.Group; }
            set
            {
                this.Model.Group = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Rate));
            }
        }

        public double Rate => this.Model.Rate;

        public int Available => this.Model.Available;

        public EmployeeViewModel()
        {
            this.Model = new Employee();
        }

        public EmployeeViewModel(Employee model)
        {
            this.Model = model;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}