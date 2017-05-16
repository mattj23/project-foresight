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
    /// Interaction logic for FixedCostView.xaml
    /// </summary>
    public partial class FixedCostView : UserControl
    {

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ProjectViewModel), typeof(FixedCostView), new PropertyMetadata(default(ProjectViewModel)));

        public static readonly DependencyProperty CategoryBoxTextProperty = DependencyProperty.Register(
            "CategoryBoxText", typeof(string), typeof(FixedCostView), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SelectedCostItemProperty = DependencyProperty.Register(
            "SelectedCostItem", typeof(FixedCostViewModel), typeof(FixedCostView), new PropertyMetadata(default(FixedCostViewModel)));

        public FixedCostViewModel SelectedCostItem
        {
            get { return (FixedCostViewModel) GetValue(SelectedCostItemProperty); }
            set { SetValue(SelectedCostItemProperty, value); }
        }
        public string CategoryBoxText
        {
            get { return (string) GetValue(CategoryBoxTextProperty); }
            set { SetValue(CategoryBoxTextProperty, value); }
        }

        public ProjectViewModel ViewModel
        {
            get { return (ProjectViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public FixedCostView()
        {
            InitializeComponent();
        }

        private void AddCategoryClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.CategoryBoxText))
            {
                this.AddItemInCategory(this.CategoryBoxText);
            }
        }

        private void AddItemInCategory(string category)
        {
            var newItem = new FixedCostViewModel { Category = category };
            this.ViewModel.FixedCosts.Add(newItem);
            this.SelectedCostItem = newItem;
            this.CategoryBoxText = "";
            
        }

        private void CategoryAddOnClick(object sender, RoutedEventArgs e)
        {
            string category = ((Button) sender).Tag as string;
            this.AddItemInCategory(category);
        }
    }
}
