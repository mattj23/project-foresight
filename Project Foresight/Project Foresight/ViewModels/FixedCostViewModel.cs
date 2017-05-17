using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class FixedCostViewModel : INotifyPropertyChanged
    {
        private EstimateViewModel _costEstimate;
        public event PropertyChangedEventHandler PropertyChanged;

        public FixedCost Model { get; set; }

        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get { return Model.Category;}
            set
            {
                Model.Category = value;
                OnPropertyChanged();
            }
        }

        public EstimateViewModel CostEstimate
        {
            get { return _costEstimate; }
            set
            {
                if (Equals(value, _costEstimate)) return;
                _costEstimate = value;
                this.Model.CostEstimate = this.CostEstimate.Model;
                OnPropertyChanged();
            }
        }

        public Guid Id => this.Model.Id;

        public FixedCostViewModel() :this(new FixedCost()) { }

        public FixedCostViewModel(FixedCost model)
        {
            this.Model = model;
            if (this.Model.CostEstimate == null)
                this.Model.CostEstimate = new Estimate();
            this.CostEstimate = new EstimateViewModel(this.Model.CostEstimate);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}