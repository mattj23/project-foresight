using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static readonly DependencyProperty SelectedCategoryProperty = DependencyProperty.Register(
            "SelectedCategory", typeof(CategoryViewModel), typeof(OrganizationView), new PropertyMetadata(default(CategoryViewModel)));

        public static readonly DependencyProperty ColorNamesProperty = DependencyProperty.Register(
            "ColorNames", typeof(ObservableCollection<string>), typeof(OrganizationView), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> ColorNames
        {
            get { return (ObservableCollection<string>) GetValue(ColorNamesProperty); }
            set { SetValue(ColorNamesProperty, value); }
        }

        public CategoryViewModel SelectedCategory
        {
            get { return (CategoryViewModel) GetValue(SelectedCategoryProperty); }
            set { SetValue(SelectedCategoryProperty, value); }
        }

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

            this.ColorNames = new ObservableCollection<string>();
            Type colors = typeof(System.Windows.Media.Colors);
            foreach (var propertyInfo in colors.GetProperties())
            {
                this.ColorNames.Add(propertyInfo.Name);
            }
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
            this.ViewModel.Employees.Add(new EmployeeViewModel(this.ViewModel.ResourceGroups));
        }

        private void DeleteResourceGroupClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ResourceGroups.Remove(this.SelectedResourceGroupViewModel);
        }

        private void ResourceGroupComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ;
        }

        private void AddCategoryClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Categories.Add(new CategoryViewModel {Name = "New Category", ColorName = "White"});
        }

        private void DeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedCategory != null)
                this.ViewModel.Categories.Remove(this.SelectedCategory);

        }
    }
}
