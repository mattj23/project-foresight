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
    /// Interaction logic for OrganizationView.xaml
    /// </summary>
    public partial class OrganizationView : UserControl
    {

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(OrganizationViewModel), typeof(OrganizationView), new PropertyMetadata(default(OrganizationViewModel)));

        public static readonly DependencyProperty SelectedResourceGroupViewModelProperty = DependencyProperty.Register(
            "SelectedResourceGroupViewModel", typeof(ResourceGroupViewModel), typeof(OrganizationView), new PropertyMetadata(default(ResourceGroupViewModel)));

        public static readonly DependencyProperty SelectedEmployeeViewModelProperty = DependencyProperty.Register(
            "SelectedEmployeeViewModel", typeof(EmployeeViewModel), typeof(OrganizationView), new PropertyMetadata(default(EmployeeViewModel)));

        public EmployeeViewModel SelectedEmployeeViewModel
        {
            get { return (EmployeeViewModel) GetValue(SelectedEmployeeViewModelProperty); }
            set { SetValue(SelectedEmployeeViewModelProperty, value); }
        }

        public ResourceGroupViewModel SelectedResourceGroupViewModel
        {
            get { return (ResourceGroupViewModel) GetValue(SelectedResourceGroupViewModelProperty); }
            set { SetValue(SelectedResourceGroupViewModelProperty, value); }
        }

        public OrganizationViewModel ViewModel
        {
            get { return (OrganizationViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public OrganizationView()
        {
            InitializeComponent();
        }

        private void AddResourceGroupClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ResourceGroups.Add(new ResourceGroupViewModel());
        }

        private void DeleteEmployeeClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Employees.Remove(this.SelectedEmployeeViewModel);
        }

        private void AddEmployeeClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Employees.Add(new EmployeeViewModel());
        }

        private void DeleteResourceGroupClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ResourceGroups.Remove(this.SelectedResourceGroupViewModel);
        }
    }
}
