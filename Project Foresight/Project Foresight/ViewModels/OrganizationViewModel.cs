using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class OrganizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Organization Model { get; }

        public string Name
        {
            get { return this.Model.Name; }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeViewModel> Employees { get; }
        public ObservableCollection<ResourceGroupViewModel> ResourceGroups { get; }

        public OrganizationViewModel() : this(new Organization()) { }

        public OrganizationViewModel(Organization model)
        {
            this.Model = model;
            this.Employees = new ObservableCollection<EmployeeViewModel>();
            this.ResourceGroups = new ObservableCollection<ResourceGroupViewModel>();

            // Synchronize the employees and resource groups
            foreach (var modelResourceGroup in this.Model.ResourceGroups)
                this.ResourceGroups.Add(new ResourceGroupViewModel(modelResourceGroup));

            foreach (var modelEmployee in this.Model.Employees)
                this.Employees.Add(new EmployeeViewModel(modelEmployee));

            // Subscribe to the ObservableCollection to keep the model synchronized
            this.Employees.CollectionChanged += EmployeesOnCollectionChanged;
            this.ResourceGroups.CollectionChanged += ResourceGroupsOnCollectionChanged;
        }

        private void ResourceGroupsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs n)
        {
            foreach (object newItem in n.NewItems)
            {
                this.Model.ResourceGroups.Add((newItem as ResourceGroupViewModel).Model);
            }
            foreach (object oldItem in n.OldItems)
            {
                if (!this.ResourceGroups.Contains(oldItem as ResourceGroupViewModel))
                {
                    this.Model.ResourceGroups.Remove((oldItem as ResourceGroupViewModel).Model);
                }
            }
        }

        private void EmployeesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs n)
        {
            foreach (object newItem in n.NewItems)
            {
                this.Model.Employees.Add((newItem as EmployeeViewModel).Model);
            }
            foreach (object oldItem in n.OldItems)
            {
                if (!this.Employees.Contains(oldItem as EmployeeViewModel))
                {
                    this.Model.Employees.Remove((oldItem as EmployeeViewModel).Model);
                }
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}