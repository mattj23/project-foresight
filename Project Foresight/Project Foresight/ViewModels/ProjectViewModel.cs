using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        public static CategoryViewModel EmptyCategory = new CategoryViewModel{Name = "No Task Category", ColorName = "White"};

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SimulationInvalidated;

        private Foresight.Project _project = null;
        private TaskViewModel _selectedTask;
        private OrganizationViewModel _organization;
        private bool _isSimulationDataValid;
        private TaskViewModel _mouseOverTask;

        public Project Model => _project;

        public string Name
        {
            get { return this._project.Name; }
            set
            {
                this._project.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return this._project.Description; }
            set
            {
                this._project.Description = value;
                OnPropertyChanged();
            }
        }

        public OrganizationViewModel Organization
        {
            get { return _organization; }
            set
            {
                if (Equals(value, _organization)) return;
                _organization = value;
                OnPropertyChanged();
            }
        }

        public TaskViewModel SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (Equals(value, _selectedTask)) return;

                // Unset the selected flags
                if (_selectedTask != null)
                {
                    _selectedTask.IsSelected = false;
                    foreach (var selectedTaskAncestor in _selectedTask.Ancestors)
                    {
                        this.TasksById[selectedTaskAncestor].IsSelectedAncestor = false;
                    }
                    foreach (var selectedTaskDescendant in _selectedTask.Descendants)
                    {
                        this.TasksById[selectedTaskDescendant].IsSelectedDescendant = false;
                    }
                }

                _selectedTask = value;

                _selectedTask.IsSelected = true;
                foreach (var selectedTaskAncestor in _selectedTask.Ancestors)
                {
                    this.TasksById[selectedTaskAncestor].IsSelectedAncestor = true;
                }
                foreach (var selectedTaskDescendant in _selectedTask.Descendants)
                {
                    this.TasksById[selectedTaskDescendant].IsSelectedDescendant = true;
                }

                OnPropertyChanged();
            }
        }

        public TaskViewModel MouseOverTask
        {
            get { return _mouseOverTask; }
            set
            {
                if (Equals(value, _mouseOverTask)) return;
                _mouseOverTask = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMouseOverATask));
            }
        }

        public bool IsMouseOverATask => this.MouseOverTask != null;

        public bool IsSimulationDataValid
        {
            get { return _isSimulationDataValid; }
            set
            {
                if (value == _isSimulationDataValid) return;
                _isSimulationDataValid = value;

                if (!_isSimulationDataValid)
                    SimulationInvalidated?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaskViewModel> Tasks { get; set; }
        public ObservableCollection<LinkViewModel> Links { get; set; }
        public ObservableCollection<FixedCostViewModel> FixedCosts { get; set; }

        private Dictionary<Guid, TaskViewModel> TasksById { get; set; }

        public ProjectViewModel()
        {
            this._project = new Project();
            this.Tasks = new ObservableCollection<TaskViewModel>();
            this.TasksById = new Dictionary<Guid, TaskViewModel>();
            this.Links = new ObservableCollection<LinkViewModel>();
            this.FixedCosts = new ObservableCollection<FixedCostViewModel>();
            this.FixedCosts.CollectionChanged += FixedCosts_CollectionChanged;
            if (this._project.Organization == null)
                this._project.Organization = new Organization();
            this.Organization = new OrganizationViewModel(this._project.Organization);
        }

        private void FixedCosts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                // Synchronize any new items with the underlying project
                foreach (var fixedCost in e.NewItems)
                {
                    this.Model.FixedCosts.Add(((FixedCostViewModel) fixedCost).Model);
                }
            }
            if (e.OldItems != null)
            {
                // Synchronize any old items with the underlying project
                foreach (var fixedCost in e.OldItems)
                {
                    var fixedCostVm = (FixedCostViewModel) fixedCost;
                    if (!this.FixedCosts.Contains(fixedCostVm))
                        this.Model.FixedCosts.Remove(fixedCostVm.Model);
                }

            }
        }

        public void AddTask(TaskViewModel task)
        {
            task.Parent = this;
            task.DependentDataChanged += Task_DependentDataChanged;
            this._project.AddTask(task.Model);
            this.Tasks.Add(task);
            this.TasksById.Add(task.Id, task);
            this.IsSimulationDataValid = false;
            if (task.Category == null)
                task.Category = EmptyCategory;
        }

        private void Task_DependentDataChanged(object sender, EventArgs e)
        {
            this.IsSimulationDataValid = false;
        }

        public TaskViewModel GetTaskById(Guid id)
        {
            return this.TasksById[id];
        }

        public void AddLink(TaskViewModel ancestor, TaskViewModel descendant)
        {
            try
            {
                ancestor.LinkToDescendant(descendant);
                Links.Add(new LinkViewModel {Start = ancestor, End = descendant});
                this.IsSimulationDataValid = false;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
        }

        public void SplitTask(TaskViewModel task)
        {
            var newTask = new TaskViewModel
            {
                X = task.X + 300,
                Y = task.Y,
                Name = task.Name + " (Cont)",
                Description = task.Description,
                Category = task.Category

            };

            this.AddTask(newTask);

            foreach (var taskDescendant in task.Descendants)
            {
                this.RemoveLink(task, TasksById[taskDescendant]);
                this.AddLink(newTask, TasksById[taskDescendant]);
            }

            this.AddLink(task, newTask);


        }

        public void RemoveTask(TaskViewModel task)
        {
            this.IsSimulationDataValid = false;

            // Remove links first
            foreach (var ancestorId in task.Ancestors)
                this.RemoveLink(this.TasksById[ancestorId], task);

            foreach (var descendantId in task.Descendants)
                this.RemoveLink(task, this.TasksById[descendantId]);

            // Remove the task
            this.TasksById.Remove(task.Id);
            this.Tasks.Remove(task);
            this.Model.RemoveTask(task.Model);
        }

        public void RemoveLink(TaskViewModel ancestor, TaskViewModel descendant)
        {
            try
            {
                this.IsSimulationDataValid = false;
                ancestor.UnlinkFromDescendant(descendant);
                var removeElement = this.Links.FirstOrDefault(x => x.Start == ancestor && x.End == descendant);
                if (removeElement != null)
                    this.Links.Remove(removeElement);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
            
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}