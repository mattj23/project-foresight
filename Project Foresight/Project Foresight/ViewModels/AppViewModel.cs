using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

                // Unlink the existing change event handler
                if (_project != null)
                {
                    _project.JournalDataChanged -= ProjectOnJournalDataChanged;
                    _project.SimulationInvalidated -= ProjectOnSimulationInvalidated;
                }

                _project = value;

                // Link the new object to the change handler
                _project.JournalDataChanged += ProjectOnJournalDataChanged;
                _project.SimulationInvalidated += ProjectOnSimulationInvalidated;
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

        public ICommand UndoCommand => new RelayCommand(RestoreFromUndoStep);
        public ICommand QuitCommand => new RelayCommand(Application.Current.Shutdown);

        public ICommand RunSimulationCommand => new RelayCommand(this.SimulationTool.RunMonteCarloSimulation);

        public SimulationToolViewModel SimulationTool { get; set; }

        public ObservableCollection<NotificationViewModel> Notifications { get; set; }

        private List<SerializableProjectViewModel> _undoChain;
        private SerializableProjectViewModel _lastProjectState;

        public AppViewModel()
        {
            this.LoadedProjectPath = "";

            // Load the project and set the last undo state
            this.Project = new ProjectViewModel {Name = "Hello world!"};
            this._lastProjectState = SerializableProjectViewModel.FromProjectViewModel(this.Project);
            this._undoChain = new List<SerializableProjectViewModel>();

            this.SimulationTool = new SimulationToolViewModel(this);
            this.SimulationTool.SimulationComplete += SimulationToolOnSimulationComplete;
            this.Notifications = new ObservableCollection<NotificationViewModel>();

        }

        private void SimulationToolOnSimulationComplete(object sender, EventArgs eventArgs)
        {
            this.AddNotification($"Monte-Carlo simulation completed in {SimulationTool.SimulationTime:N1} seconds", 3, new SolidColorBrush(Colors.Yellow));
        }

        public void AddNotification(string message, double displaySeconds, SolidColorBrush brush = null)
        {
            var notification = new NotificationViewModel()
            {
                Message = message,
                Color = brush ?? new SolidColorBrush(Colors.WhiteSmoke)
            };
            this.Notifications.Add(notification);
            DelayedAction.Execute(() => this.RemoveNotification(notification), (int) (displaySeconds * 1000));
        }

        public void RemoveNotification(NotificationViewModel notification)
        {
            notification.IsRemoving = true;
            DelayedAction.Execute(() => this.Notifications.Remove(notification), 1000);
        }

        public void SaveProject()
        {
            if (string.IsNullOrEmpty(this.LoadedProjectPath) || !File.Exists(this.LoadedProjectPath))
            {
                this.SaveProjectAs();
            }
            else
            {
                File.WriteAllText(this.LoadedProjectPath, JsonConvert.SerializeObject(SerializableProjectViewModel.FromProjectViewModel(this.Project), Formatting.Indented));
                this.AddNotification($"Saved project '{Path.GetFileName(this.LoadedProjectPath)}'", 5, new SolidColorBrush(Colors.Aqua));

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
                File.WriteAllText(this.LoadedProjectPath, JsonConvert.SerializeObject(SerializableProjectViewModel.FromProjectViewModel(this.Project), Formatting.Indented));
                this.AddNotification($"Saved project '{Path.GetFileName(dialog.FileName)}'", 5, new SolidColorBrush(Colors.Aqua));
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
                var working = JsonConvert.DeserializeObject<SerializableProjectViewModel>(text);
                this.Project = SerializableProjectViewModel.ToProjectViewModel(working);
                this._lastProjectState = SerializableProjectViewModel.FromProjectViewModel(this.Project);
                this.AddNotification($"Opened project '{Path.GetFileName(dialog.FileName)}'", 5, new SolidColorBrush(Colors.LightGreen));
                this._undoChain.Clear();
            }
        }

        private void ProjectOnJournalDataChanged(object sender, EventArgs eventArgs)
        {
            if (!sender.Equals(this.Project))
                throw new ArgumentException("For some reason the event was thrown on an object that isn't this Project");
            this.SaveUndoStep();
        }


        private void SaveUndoStep()
        {
            this._undoChain.Add(_lastProjectState);
            _lastProjectState = SerializableProjectViewModel.FromProjectViewModel(this.Project);
            if (this._undoChain.Count > 30)
            {
                this._undoChain.RemoveAt(0);
            }
        }


        private void RestoreFromUndoStep()
        {
            if (this._undoChain.Any())
            {
                _lastProjectState = this._undoChain.Last();
                this._undoChain.Remove(_lastProjectState);
                this.Project = SerializableProjectViewModel.ToProjectViewModel(_lastProjectState);
            }

        }

        private void ProjectOnSimulationInvalidated(object sender, EventArgs eventArgs)
        {
            this.SimulationTool.Invalidate();
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}