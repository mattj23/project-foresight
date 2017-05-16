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
        public event PropertyChangedEventHandler PropertyChanged;

        private Foresight.Project _project = null;
        private TaskViewModel _selectedTask;
        private OrganizationViewModel _organization;
        private bool _isSimulationDataValid;

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

        public bool IsSimulationDataValid
        {
            get { return _isSimulationDataValid; }
            set
            {
                if (value == _isSimulationDataValid) return;
                _isSimulationDataValid = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaskViewModel> Tasks { get; set; }
        public ObservableCollection<LinkViewModel> Links { get; set; }

        private Dictionary<Guid, TaskViewModel> TasksById { get; set; }

        public ProjectViewModel()
        {
            this._project = new Project();
            this.Tasks = new ObservableCollection<TaskViewModel>();
            this.TasksById = new Dictionary<Guid, TaskViewModel>();
            this.Links = new ObservableCollection<LinkViewModel>();

            if (this._project.Organization == null)
                this._project.Organization = new Organization();
            this.Organization = new OrganizationViewModel(this._project.Organization);
        }

        public void AddTask(TaskViewModel task)
        {
            task.Parent = this;
            task.DependentDataChanged += Task_DependentDataChanged;
            this._project.AddTask(task.Model);
            this.Tasks.Add(task);
            this.TasksById.Add(task.Id, task);
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