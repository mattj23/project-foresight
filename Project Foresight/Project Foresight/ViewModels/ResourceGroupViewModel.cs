using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foresight;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class ResourceGroupViewModel : INotifyPropertyChanged, IResource
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ResourceGroup Model { get; }

        public string Name
        {
            get
            {
                return this.Model?.Name;
            }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged();
            }
        }

        public double Rate
        {
            get { return this.Model.Rate;}
            set
            {
                this.Model.Rate = value;
                OnPropertyChanged();
            }
        }

        public int Available
        {
            get { return this.Model.Available; }
            set
            {
                this.Model.Available = value;
                OnPropertyChanged();
            }
        }

        public ResourceGroupViewModel()
        {
            this.Model = new ResourceGroup();
        }

        public ResourceGroupViewModel(ResourceGroup model)
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