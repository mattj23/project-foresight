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

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TaskViewModel), typeof(TaskView), new PropertyMetadata(default(TaskViewModel)));

        public static readonly DependencyProperty LayoutElementProperty = DependencyProperty.Register(
            "LayoutElement", typeof(IInputElement), typeof(TaskView), new PropertyMetadata(default(IInputElement)));

        public static readonly DependencyProperty IsSelectingResourceProperty = DependencyProperty.Register(
            "IsSelectingResource", typeof(bool), typeof(TaskView), new PropertyMetadata(default(bool)));

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
            if (e.Key == Key.Enter)
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

        private void AddResourceOnClick(object sender, RoutedEventArgs e)
        {
            this.IsSelectingResource = !this.IsSelectingResource;
            this.BringToFront();
        }

        private void SelectorDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedName = (sender as ListBox).SelectedItem as string;
            var locatedResource = this.ViewModel.Parent.Organization.FindResourceByName(selectedName);
            if (locatedResource != null)
                this.ViewModel.Resources.Add(locatedResource);
            this.IsSelectingResource = false;
        }

        private void DeleteResourceOnClick(object sender, RoutedEventArgs e)
        {
            var resource = ((Button) sender).Tag as IResource;
            this.ViewModel.Resources.Remove(resource);
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
    }
}
