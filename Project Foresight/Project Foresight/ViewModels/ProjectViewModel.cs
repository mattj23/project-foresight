using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ObservableCollection<TaskViewModel> Tasks { get; set; }
        public ObservableCollection<LinkViewModel> Links { get; set; }

        private Dictionary<Guid, TaskViewModel> TasksById { get; set; }

        public ProjectViewModel()
        {
            this._project = new Project();
            this.Tasks = new ObservableCollection<TaskViewModel>();
            this.TasksById = new Dictionary<Guid, TaskViewModel>();
            this.Links = new ObservableCollection<LinkViewModel>();
        }

        public void AddTask(TaskViewModel task)
        {
            task.Parent = this;
            this._project.AddTask(task.Model);
            this.Tasks.Add(task);
            this.TasksById.Add(task.Id, task);
        }

        public void AddLink(TaskViewModel ancestor, TaskViewModel descendant)
        {
            try
            {
                ancestor.LinkToDescendant(descendant);
                Links.Add(new LinkViewModel {Start = ancestor, End = descendant});
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void RemoveTask(TaskViewModel task)
        {
            throw new NotImplementedException();
        }

        public void RemoveLink(TaskViewModel ancestor, TaskViewModel descendant)
        {
            throw new NotImplementedException();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}