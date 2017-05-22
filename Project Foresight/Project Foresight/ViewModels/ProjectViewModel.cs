using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;
using Project_Foresight.Serialization;

namespace Project_Foresight.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        public static CategoryViewModel EmptyCategory = new CategoryViewModel{Name = "No Task Category", ColorName = "White"};

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SimulationInvalidated;
        public event EventHandler JournalDataChanged;

        private Foresight.Project _project = null;
        private TaskViewModel _selectedTask;
        private OrganizationViewModel _organization;
        private bool _isSimulationDataValid;
        private TaskViewModel _mouseOverTask;

        private bool _suppressChangeAlert = false;

        public Project Model => _project;

        public string Name
        {
            get { return this._project.Name; }
            set
            {
                this._project.Name = value;
                OnPropertyChanged();
                this.RaiseJournalChangeAlert();
            }
        }

        public string Description
        {
            get { return this._project.Description; }
            set
            {
                this._project.Description = value;
                OnPropertyChanged();
                this.RaiseJournalChangeAlert();

            }
        }

        public OrganizationViewModel Organization
        {
            get { return _organization; }
            set
            {
                if (Equals(value, _organization)) return;
                if (_organization != null)
                    _organization.JournalDataChanged -= OrganizationOnJournalDataChanged;
                _organization = value;
                _organization.JournalDataChanged += OrganizationOnJournalDataChanged;
                OnPropertyChanged();
                this.RaiseJournalChangeAlert();

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
                if (!value)
                    SimulationInvalidated?.Invoke(this, EventArgs.Empty);

                if (value == _isSimulationDataValid) return;
                _isSimulationDataValid = value;


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
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddTask(TaskViewModel task)
        {
            task.Parent = this;
            task.SimulationDataChanged += TaskSimulationDataChanged;
            task.JournalDataChanged += TaskOnJournalDataChanged;
            this._project.AddTask(task.Model);
            this.Tasks.Add(task);
            this.TasksById.Add(task.Id, task);
            this.IsSimulationDataValid = false;
            if (task.Category == null)
                task.Category = EmptyCategory;

            this.RaiseJournalChangeAlert();
        }

        private void TaskOnJournalDataChanged(object sender, EventArgs eventArgs)
        {
            this.RaiseJournalChangeAlert();
        }

        private void TaskSimulationDataChanged(object sender, EventArgs e)
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
                this._suppressChangeAlert = true;
                ancestor.LinkToDescendant(descendant);
                Links.Add(new LinkViewModel {Start = ancestor, End = descendant});
                this.IsSimulationDataValid = false;
                this.RaiseJournalChangeAlert();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                this._suppressChangeAlert = false;
            }
        }

        public void SplitTask(TaskViewModel task)
        {
            this._suppressChangeAlert = true;

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

            this._suppressChangeAlert = false;
            this.RaiseJournalChangeAlert();
        }

        public void RemoveTask(TaskViewModel task)
        {
            this._suppressChangeAlert = true;

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

            this._suppressChangeAlert = false;
            this.RaiseJournalChangeAlert();
        }

        public void RemoveLink(TaskViewModel ancestor, TaskViewModel descendant)
        {
            try
            {
                this._suppressChangeAlert = true;
                this.IsSimulationDataValid = false;

                ancestor.UnlinkFromDescendant(descendant);
                var removeElement = this.Links.FirstOrDefault(x => x.Start == ancestor && x.End == descendant);

                if (removeElement != null)
                    this.Links.Remove(removeElement);

                this._suppressChangeAlert = false;
                this.RaiseJournalChangeAlert();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                this._suppressChangeAlert = false;
            }
        }


        private void OrganizationOnJournalDataChanged(object sender, EventArgs eventArgs)
        {
            this.RaiseJournalChangeAlert();
        }

        private void RaiseJournalChangeAlert()
        {
            if (!_suppressChangeAlert)
                this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public ProjectViewModel DeepCopy()
        {
            return ProjectViewModel.DeepCopy(this);
        }

        /// <summary>
        /// Create a deep copy of a ProjectViewModel, including new objects rather than links
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ProjectViewModel DeepCopy(ProjectViewModel project)
        {
            var serialized = SerializableProjectViewModel.FromProjectViewModel(project);
            return SerializableProjectViewModel.ToProjectViewModel(serialized);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //this.RaiseJournalChangeAlert();
        }
    }
}