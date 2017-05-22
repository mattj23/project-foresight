using System;
using System.Collections.ObjectModel;
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
        public event EventHandler SimulationDataChanged;
        public event EventHandler JournalDataChanged;

        private Foresight.PertTask _task = null;
        private double _y;
        private double _x;
        private double _zIndex;
        private bool _isSelected;
        private bool _isSelectedAncestor;
        private bool _isSelectedDescendant;
        private TaskSimulationData _simulatedData;
        private bool _isMouseOver;
        private CategoryViewModel _category;

        #region View-related Properties
        public Point CenterPoint => new Point(this.X, this.Y);

        public double X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = Math.Round(value / 10) * 10;
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
                _y = Math.Round(value / 10) * 10;
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

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set
            {
                if (value == _isMouseOver) return;
                _isMouseOver = value;
                OnPropertyChanged();
            }
        }

        public ProjectViewModel Parent { get; set; }

        public TaskSimulationData SimulatedData
        {
            get { return _simulatedData; }
            set
            {
                if (Equals(value, _simulatedData)) return;
                _simulatedData = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Model-related Properties
        public string Name
        {
            get { return this._task.Name; }
            set
            {
                this._task.Name = value;
                OnPropertyChanged();
                this.JournalDataChanged?.Invoke(this, EventArgs.Empty);

            }
        }

        public string Description
        {
            get { return this._task.Description; }
            set
            {
                this._task.Description = value;
                this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public CategoryViewModel Category
        {
            get { return _category; }
            set
            {
                if (Equals(value, _category)) return;
                _category = value;
                this.Model.Category = value.Name;
                this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IResource> Resources { get; }

        public EstimateViewModel TimeEstimate { get; }

        public PertTask Model => this._task;

        public Guid[] Ancestors => this._task.Ancestors.Select(x => x.Id).ToArray();

        public Guid[] Descendants => this._task.Descendants.Select(x => x.Id).ToArray();

        public Guid[] AllAncestors => this._task.AllAncestors.Select(x => x.Id).ToArray();

        public Guid[] AllDescendants => this._task.AllDescendants.Select(x => x.Id).ToArray();

        public Guid Id => this._task.Id;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskViewModel() : this(new PertTask())
        {
        }

        public TaskViewModel(PertTask taskModel)
        {
            this._task = taskModel;
            this.Resources = new ObservableCollection<IResource>();
            foreach (var modelResource in this.Model.Resources)
            {
                this.Resources.Add(modelResource);
            }
            this.Resources.CollectionChanged += Resources_CollectionChanged;
            this.TimeEstimate = new EstimateViewModel(this._task.TimeEstimate);
            this.TimeEstimate.PropertyChanged += TimeEstimate_PropertyChanged;
        }

        private void TimeEstimate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);

        }

        private void Resources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);

            this.SynchronizeResources();
        }

        private void SynchronizeResources()
        {
            this.Model.Resources.Clear();
            foreach (var resource in this.Resources)
            {
                this.Model.Resources.Add(resource);
            }
        }

        public void LinkToAncestor(TaskViewModel ancestor)
        {
            this._task.LinkToAncestor(ancestor._task);
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LinkToDescendant(TaskViewModel descendant)
        {
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this._task.LinkToDescendant(descendant._task);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UnlinkFromAncestor(TaskViewModel ancestor)
        {
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this._task.UnlinkFromAncestor(ancestor._task);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UnlinkFromDescendant(TaskViewModel descendant)
        {
            this.SimulationDataChanged?.Invoke(this, EventArgs.Empty);
            this._task.UnlinkFromDescendant(descendant._task);
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UnlinkAll()
        {
            this._task.UnlinkAll();
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReportJournalDataChanged()
        {
            this.JournalDataChanged?.Invoke(this, EventArgs.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}