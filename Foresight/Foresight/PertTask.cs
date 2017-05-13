using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight.Annotations;

namespace Foresight
{
    /// <summary>
    /// A PertTask represents a task in a PERT network.
    /// </summary>
    public class PertTask : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        
        public ObservableCollection<Employee> Employees { get; set; }

        private HashSet<PertTask> Ancestors { get; set; }
        private HashSet<PertTask> Descendants { get; set; }

        public PertTask()
        {
            this.Employees = new ObservableCollection<Employee>();
            this.Ancestors = new HashSet<PertTask>();
            this.Descendants = new HashSet<PertTask>();
        }

        public void LinkToAncestor(PertTask ancestor)
        {
            this.Ancestors.Add(ancestor);
            ancestor.Descendants.Add(this);
        }

        public void LinkToDescendant(PertTask descendant)
        {
            this.Descendants.Add(descendant);
            descendant.Ancestors.Add(this);
        }

        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}