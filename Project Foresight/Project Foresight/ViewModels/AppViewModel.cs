using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Project_Foresight.Annotations;
using Project_Foresight.Serialization;
using Project_Foresight.Tools;

namespace Project_Foresight.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private ProjectViewModel _project;
        private string _loadedProjectPath;
        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectViewModel Project
        {
            get { return _project; }
            set
            {
                if (Equals(value, _project)) return;
                _project = value;
                OnPropertyChanged();
            }
        }

        public string LoadedProjectPath
        {
            get { return _loadedProjectPath; }
            set
            {
                if (value == _loadedProjectPath) return;
                _loadedProjectPath = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand => new RelayCommand(SaveProject);
        public ICommand SaveAsCommand => new RelayCommand(SaveProjectAs);
        public ICommand OpenCommand => new RelayCommand(OpenProject);

        public SimulationToolViewModel SimulationTool { get; set; }

        public AppViewModel()
        {
            this.LoadedProjectPath = "";
            this.Project = new ProjectViewModel {Name = "Hello world!"};
            this.SimulationTool = new SimulationToolViewModel(this);

        }

        public void SaveProject()
        {
            if (string.IsNullOrEmpty(this.LoadedProjectPath) || !File.Exists(this.LoadedProjectPath))
            {
                this.SaveProjectAs();
            }
            else
            {
                File.WriteAllText(this.LoadedProjectPath, JsonConvert.SerializeObject(SerializeableProjectViewModel.FromProjectViewModel(this.Project), Formatting.Indented));
            }
        }

        public void SaveProjectAs()
        {
            
            var dialog = new SaveFileDialog
            {
                Filter = "Project Foresight JSON File (.prfj) | *.prfj",
                DefaultExt = ".prfj",
                FileName = this.LoadedProjectPath
            };

            if (dialog.ShowDialog() == true)
            {
                this.LoadedProjectPath = dialog.FileName;
                File.WriteAllText(this.LoadedProjectPath, JsonConvert.SerializeObject(SerializeableProjectViewModel.FromProjectViewModel(this.Project), Formatting.Indented));
            }

        }

        public void OpenProject()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Project Foresight JSON File (.prfj) | *.prfj",
                DefaultExt = ".prfj",
                FileName = this.LoadedProjectPath
            };

            if (dialog.ShowDialog() == true)
            {
                this.LoadedProjectPath = dialog.FileName;
                var text = File.ReadAllText(this.LoadedProjectPath);
                var working = JsonConvert.DeserializeObject<SerializeableProjectViewModel>(text);
                this.Project = SerializeableProjectViewModel.ToProjectViewModel(working);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}