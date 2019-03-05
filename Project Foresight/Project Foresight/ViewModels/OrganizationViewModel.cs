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
        public event EventHandler JournalDataChanged;

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


        /// <summary>
        /// All resource names, employees and groups
        /// </summary>
        public ObservableCollection<string> ResourceNames { get; set; }

        public ObservableCollection<CategoryViewModel> Categories { get; set; }

        public OrganizationViewModel() : this(new Organization()) { }

        public OrganizationViewModel(Organization model)
        {
            this.Model = model;
            this.Employees = new ObservableCollection<EmployeeViewModel>();
            this.ResourceGroups = new ObservableCollection<ResourceGroupViewModel>();
            this.ResourceNames = new ObservableCollection<string>();
            this.Categories = new ObservableCollection<CategoryViewModel>();

            // Synchronize the employees and resource groups
            foreach (var modelResourceGroup in this.Model.ResourceGroups)
                this.ResourceGroups.Add(new ResourceGroupViewModel(modelResourceGroup));

            foreach (var employeeModel in this.Model.Employees)
            {
                var newEmployee = new EmployeeViewModel(employeeModel, this.ResourceGroups);
                this.Employees.Add(newEmployee);
            }

            // Subscribe to the ObservableCollection to keep the resource names synchronized
            this.ResourceGroups.CollectionChanged += SynchronizeCollections;
            this.Employees.CollectionChanged += SynchronizeCollections;
            this.SynchronizeCollections(null, null);
        }

        private void SynchronizeCollections(object sender, NotifyCollectionChangedEventArgs e)
        {
            //var all = this.ResourceGroups.Select(x => x.Name).Concat(this.Employees.Select(x => x.Name)).ToArray();
            var all = this.Employees.Select(x => x.Name).ToArray();
            var remove = this.ResourceNames.Where(ex => !all.Contains(ex)).ToArray();
            foreach (var s in remove)
            {
                this.ResourceNames.Remove(s);
            }

            var missing = all.Where(a => !this.ResourceNames.Contains(a)).ToArray();
            foreach (var s in missing)
            {
                this.ResourceNames.Add(s);
            }

        }


        public IResource FindResourceByName(string name)
        {
            var employeeResource = this.Employees.FirstOrDefault(x => x.Name == name);
            if (employeeResource != null)
                return employeeResource;
            return this.ResourceGroups.FirstOrDefault(x => x.Name == name);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}