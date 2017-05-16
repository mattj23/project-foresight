using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Views
{
    /// <summary>
    /// Interaction logic for SimulationView.xaml
    /// </summary>
    public partial class SimulationView : UserControl
    {

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(SimulationToolViewModel), typeof(SimulationView), new PropertyMetadata(default(SimulationToolViewModel)));


        public SimulationToolViewModel ViewModel
        {
            get { return (SimulationToolViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public SimulationView()
        {
            InitializeComponent();
        }

        private void RunSimulationClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.RunMonteCarloSimulation();
        }
    }
}
