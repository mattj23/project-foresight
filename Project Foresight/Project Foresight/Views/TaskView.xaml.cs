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
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        private Point _mouseDownPoint;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(TaskViewModel), typeof(TaskView), new PropertyMetadata(default(TaskViewModel)));

        public TaskViewModel ViewModel
        {
            get { return (TaskViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public TaskView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas canvas = sender as Canvas;
                Point canvasPoint = e.GetPosition(canvas);

                this.ViewModel.X += (canvasPoint.X - _mouseDownPoint.X);
                this.ViewModel.Y += (canvasPoint.Y - _mouseDownPoint.Y);
                this._mouseDownPoint = canvasPoint;
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDownPoint = e.GetPosition(canvas);
            }
        }
    }
}
