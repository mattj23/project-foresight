using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using Foresight;
using Project_Foresight.Tools;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Views
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        public event EventHandler<TaskViewModel> ResourceEditOnClick;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TaskViewModel), typeof(TaskView), new PropertyMetadata(default(TaskViewModel)));

        public static readonly DependencyProperty LayoutElementProperty = DependencyProperty.Register(
            "LayoutElement", typeof(IInputElement), typeof(TaskView), new PropertyMetadata(default(IInputElement)));

        public static readonly DependencyProperty IsSelectingResourceProperty = DependencyProperty.Register(
            "IsSelectingResource", typeof(bool), typeof(TaskView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsEditingCategoryProperty = DependencyProperty.Register(
            "IsEditingCategory", typeof(bool), typeof(TaskView), new PropertyMetadata(default(bool)));

        public bool IsEditingCategory
        {
            get { return (bool) GetValue(IsEditingCategoryProperty); }
            set { SetValue(IsEditingCategoryProperty, value); }
        }

        public bool IsSelectingResource
        {
            get { return (bool) GetValue(IsSelectingResourceProperty); }
            set { SetValue(IsSelectingResourceProperty, value); }
        }

        public IInputElement LayoutElement
        {
            get { return (IInputElement) GetValue(LayoutElementProperty); }
            set { SetValue(LayoutElementProperty, value); }
        }

        public TaskViewModel ViewModel
        {
            get { return (TaskViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public TaskView()
        {
            InitializeComponent();
        }

        private void BringToFront()
        {
            var maxZ = this.ViewModel.Parent.Tasks.Select(x => x.ZIndex).Max();
            this.ViewModel.ZIndex = maxZ + 1;
        }

        private void EditBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
                (sender as TextBox).IsReadOnly = true;
            }
        }

        private void EditBoxLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).IsReadOnly = true;
        }

        private void EditBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox) sender;
            textBox.IsReadOnly = false;

            DelayedAction.Execute(textBox.SelectAll, 50);
        }


        private void SelectorDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedName = (sender as ListBox).SelectedItem as string;
            var locatedResource = this.ViewModel.Parent.Organization.FindResourceByName(selectedName);
            if (locatedResource != null)
                this.ViewModel.Resources.Add(locatedResource);
            this.IsSelectingResource = false;
        }


        private void UserMouseControlExit(object sender, MouseEventArgs e)
        {
            this.ViewModel.IsMouseOver = false;
            this.ViewModel.Parent.MouseOverTask = null;
        }

        private void UserControlMouseEnter(object sender, MouseEventArgs e)
        {
            this.ViewModel.IsMouseOver = true;
            this.ViewModel.Parent.MouseOverTask = this.ViewModel;

        }

        private void EditCategoryOnClick(object sender, MouseButtonEventArgs e)
        {
            this.IsEditingCategory = true;
        }

        private void CategoryEditOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.IsEditingCategory = false;

                var category = (CategoryViewModel) ((Border) sender).Tag;
                this.ViewModel.Category = category;
                e.Handled = true;
            }

        }

        private void AddResourceOnClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.ResourceEditOnClick?.Invoke(this, this.ViewModel);
                this.BringToFront();
                e.Handled = true;
            }
        }

        private void DeleteResourceButtonOnClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var resource = ((Grid)sender).Tag as IResource;
                this.ViewModel.Resources.Remove(resource);
            }
        }
    }
}
