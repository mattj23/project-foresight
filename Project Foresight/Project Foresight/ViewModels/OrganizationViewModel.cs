using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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

        public string Description
        {
            get { return this.Model.Description; }
            set
            {
                this.Model.Description = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeViewModel> Employees { get; }
        public ObservableCollection<ResourceGroupViewModel> ResourceGroups { get; }

        public ObservableCollection<string> ResourceGroupNames { get; }

        public OrganizationViewModel() : this(new Organization()) { }

        public OrganizationViewModel(Organization model)
        {
            this.Model = model;
            this.Employees = new ObservableCollection<EmployeeViewModel>();
            this.ResourceGroups = new ObservableCollection<ResourceGroupViewModel>();
            this.ResourceGroupNames = new ObservableCollection<string>();

            // Synchronize the employees and resource groups
            foreach (var modelResourceGroup in this.Model.ResourceGroups)
                this.ResourceGroups.Add(new ResourceGroupViewModel(modelResourceGroup));

            foreach (var modelEmployee in this.Model.Employees)
            {
                this.AddEmployeeToModel(modelEmployee);
            }

            // Subscribe to the ObservableCollection to keep the model synchronized
            this.Employees.CollectionChanged += EmployeesOnCollectionChanged;
            this.ResourceGroups.CollectionChanged += ResourceGroupsOnCollectionChanged;
            this.SynchResourceNames();
        }

        private void AddEmployeeToModel(Employee employeeModel)
        {
            var newEmployee = new EmployeeViewModel(employeeModel);
            this.Employees.Add(newEmployee);
            newEmployee.PropertyChanged += EmployeeOnPropertyChanged;
        }

        private void EmployeeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResourceGroupName")
            {
                var employeeViewModel = sender as EmployeeViewModel;
                employeeViewModel.Group =
                    this.ResourceGroups.FirstOrDefault(x => x.Name == employeeViewModel.ResourceGroupName);
            }

        }


        private void ResourceGroupsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs n)
        {
            if (n.NewItems != null)
            {
                foreach (object newItem in n.NewItems)
                {
                    this.Model.ResourceGroups.Add((newItem as ResourceGroupViewModel).Model);
                }
            }

            if (n.OldItems != null)
            {
                foreach (object oldItem in n.OldItems)
                {
                    if (!this.ResourceGroups.Contains(oldItem as ResourceGroupViewModel))
                    {
                        this.Model.ResourceGroups.Remove((oldItem as ResourceGroupViewModel).Model);
                    }
                }
            }

            this.SynchResourceNames();
        }

        private void SynchResourceNames()
        {
            this.ResourceGroupNames.Clear();
            var sortedNames = this.ResourceGroups.Select(x => x.Name).ToList();
            sortedNames.Sort();
            foreach (var name in sortedNames)
            {
                this.ResourceGroupNames.Add(name);
            }
        }

        private void EmployeesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs n)
        {
            if (n.NewItems != null)
            {
                foreach (object newItem in n.NewItems)
                {
                    this.AddEmployeeToModel((newItem as EmployeeViewModel).Model);
                }
            }

            if (n.OldItems != null)
            {
                foreach (object oldItem in n.OldItems)
                {
                    if (!this.Employees.Contains(oldItem as EmployeeViewModel))
                    {
                        var oldViewModel = oldItem as EmployeeViewModel;
                        oldViewModel.PropertyChanged -= EmployeeOnPropertyChanged;
                        this.Model.Employees.Remove(oldViewModel.Model);
                    }
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