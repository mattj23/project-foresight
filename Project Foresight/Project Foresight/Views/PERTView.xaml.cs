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
using Project_Foresight.Tools;
using Project_Foresight.ViewModels;

namespace Project_Foresight.Views
{
    /// <summary>
    /// Interaction logic for PERTView.xaml
    /// </summary>
    public partial class PERTView : UserControl
    {
        private Point _dragStartMouse;
        private Point _dragStartShift;

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ProjectViewModel), typeof(PERTView), new PropertyMetadata(default(ProjectViewModel)));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom", typeof(double), typeof(PERTView), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ShiftXProperty = DependencyProperty.Register(
            "ShiftX", typeof(double), typeof(PERTView), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ShiftYProperty = DependencyProperty.Register(
            "ShiftY", typeof(double), typeof(PERTView), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsRadialMenuOpenProperty = DependencyProperty.Register(
            "IsRadialMenuOpen", typeof(bool), typeof(PERTView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty RadialMarginProperty = DependencyProperty.Register(
            "RadialMargin", typeof(Thickness), typeof(PERTView), new PropertyMetadata(default(Thickness)));

        public Thickness RadialMargin
        {
            get { return (Thickness) GetValue(RadialMarginProperty); }
            set { SetValue(RadialMarginProperty, value); }
        }

        public bool IsRadialMenuOpen
        {
            get { return (bool) GetValue(IsRadialMenuOpenProperty); }
            set { SetValue(IsRadialMenuOpenProperty, value); }
        }

        public double ShiftY
        {
            get { return (double) GetValue(ShiftYProperty); }
            set { SetValue(ShiftYProperty, value); }
        }

        public double ShiftX
        {
            get { return (double) GetValue(ShiftXProperty); }
            set { SetValue(ShiftXProperty, value); }
        }

        public double Zoom
        {
            get { return (double) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public ProjectViewModel ViewModel
        {
            get { return (ProjectViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public ICommand CloseRadialMenu => new RelayCommand(() => IsRadialMenuOpen = false);

        private double _minZoom = 0.5;
        private double _maxZoom = 3.0;

        public PERTView()
        {
            InitializeComponent();
            this.Zoom = 1;
        }


        private void ControlOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // When zooming, the point under the mouse should remain under the mouse after the zoom has happened
            var mousePoint = e.GetPosition(this.ViewArea);
            double xGap = (mousePoint.X - this.ShiftX) / this.Zoom;
            double yGap = (mousePoint.Y - this.ShiftY) / this.Zoom;

            this.Zoom *= 1 + (e.Delta / 1200.0);
            if (this.Zoom < this._minZoom)
                this.Zoom = this._minZoom;
            if (this.Zoom > this._maxZoom)
                this.Zoom = this._maxZoom;

            this.ShiftX = this.ValidateXShift(-1 * (xGap * this.Zoom - mousePoint.X));
            this.ShiftY = this.ValidateYShift(-1 * (yGap * this.Zoom - mousePoint.Y));
        }

        private void ControlOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Close the radial menu if it's open
            if (e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                if (this.IsRadialMenuOpen)
                    this.IsRadialMenuOpen = false;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this._dragStartShift = new Point(this.ShiftX, this.ShiftY);
                this._dragStartMouse = e.GetPosition(this.ViewArea);
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.IsRadialMenuOpen = true;
                var mousePosition = e.GetPosition(this.ViewArea);
                this.RadialMargin = new Thickness(mousePosition.X - (this.RadialMenu.Width / 2.0), mousePosition.Y - (this.RadialMenu.Height / 2.0), 0, 0);
            }
        }

        private void ControlOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this.ShiftX = this.ValidateXShift(e.GetPosition(this.ViewArea).X - this._dragStartMouse.X + this._dragStartShift.X);
                this.ShiftY = this.ValidateYShift(e.GetPosition(this.ViewArea).Y - this._dragStartMouse.Y + this._dragStartShift.Y);
            }
        }

        private double ValidateXShift(double shift)
        {
            // Validation logic goes here 
            return shift;
        }

        private double ValidateYShift(double shift)
        {
            // Validation logic goes here
            return shift;
        }

        private void AddTask_OnClick(object sender, RoutedEventArgs e)
        {
            var position = System.Windows.Input.Mouse.GetPosition(ViewCanvas);
            this.ViewModel.AddTask(new TaskViewModel {Name = "New Task", Description = "Description", X = position.X, Y=position.Y});
        }
    }
}
