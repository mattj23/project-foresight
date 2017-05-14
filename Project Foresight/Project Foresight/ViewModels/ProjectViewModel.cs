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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}