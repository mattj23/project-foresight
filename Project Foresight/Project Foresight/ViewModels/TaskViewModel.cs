using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Foresight.PertTask _task = null;
        private double _y;
        private double _x;
        private double _zIndex;
        private bool _isSelected;
        private bool _isSelectedAncestor;
        private bool _isSelectedDescendant;

        public string Name
        {
            get { return this._task.Name; }
            set
            {
                this._task.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return this._task.Description; }
            set
            {
                this._task.Description = value;
                OnPropertyChanged();
            }
        }

        public Point CenterPoint => new Point(this.X, this.Y);

        public double X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CenterPoint));
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CenterPoint));

            }
        }

        public double ZIndex
        {
            get { return _zIndex; }
            set
            {
                if (value.Equals(_zIndex)) return;
                _zIndex = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelectedAncestor
        {
            get { return _isSelectedAncestor; }
            set
            {
                if (value == _isSelectedAncestor) return;
                _isSelectedAncestor = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelectedDescendant
        {
            get { return _isSelectedDescendant; }
            set
            {
                if (value == _isSelectedDescendant) return;
                _isSelectedDescendant = value;
                OnPropertyChanged();
            }
        }

        public ProjectViewModel Parent { get; set; }

        public PertTask Model => this._task;

        public Guid[] Ancestors => this._task.Ancestors.Select(x => x.Id).ToArray();

        public Guid[] Descendants => this._task.Descendants.Select(x => x.Id).ToArray();

        public Guid[] AllAncestors => this._task.AllAncestors.Select(x => x.Id).ToArray();

        public Guid[] AllDescendants => this._task.AllDescendants.Select(x => x.Id).ToArray();

        public Guid Id => this._task.Id;


        public TaskViewModel()
        {
            this._task = new PertTask();
        }

        public TaskViewModel(PertTask taskModel)
        {
            this._task = taskModel;
        }

        public void LinkToAncestor(TaskViewModel ancestor)
        {
            this._task.LinkToAncestor(ancestor._task);
        }

        public void LinkToDescendant(TaskViewModel descendant)
        {
            this._task.LinkToDescendant(descendant._task);
        }

        public void UnlinkFromAncestor(TaskViewModel ancestor)
        {
            this._task.UnlinkFromAncestor(ancestor._task);
        }

        public void UnlinkFromDescendant(TaskViewModel descendant)
        {
            this._task.UnlinkFromDescendant(descendant._task);
        }

        public void UnlinkAll()
        {
            this._task.UnlinkAll();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}