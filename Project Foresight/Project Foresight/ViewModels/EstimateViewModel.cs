using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class EstimateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Estimate Model { get; }

        public double Min
        {
            get { return this.Model.Min; }
            set
            {
                this.Model.Min = value;
                OnPropertyChanged();

                if (this.Min > this.Mode)
                    this.Mode = this.Min;
            }
        }

        public double Max
        {
            get { return this.Model.Max; }
            set
            {
                this.Model.Max = value;
                OnPropertyChanged();

                if (this.Max < this.Mode)
                    this.Mode = this.Max;
            }
        }

        public double Mode
        {
            get { return this.Model.Mode; }
            set
            {
                this.Model.Mode = value;
                OnPropertyChanged();

                if (this.Mode < this.Min)
                    this.Min = this.Mode;
                if (this.Mode > this.Max)
                    this.Max = this.Mode;
            }
        }
        public EstimateViewModel()
        {
            this.Model = new Estimate();
        }

        public EstimateViewModel(Estimate model)
        {
            this.Model = model;
        }
        


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}