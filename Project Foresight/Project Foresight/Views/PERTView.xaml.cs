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
        public enum PertViewMode
        {
            Normal,
            AddTask,
            AddLink,
            RemoveTask,
            RemoveLink
        }

        private Point _dragStartMouse;
        private Point _dragStartShift;

        #region Dependency Properties
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

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(PertViewMode), typeof(PERTView), new PropertyMetadata(default(PertViewMode)));

        public static readonly DependencyProperty CanvasMousePointProperty = DependencyProperty.Register(
            "CanvasMousePoint", typeof(Point), typeof(PERTView), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register(
            "ToolTipText", typeof(string), typeof(PERTView), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsLinkEditDisplayedProperty = DependencyProperty.Register(
            "IsLinkEditDisplayed", typeof(bool), typeof(PERTView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty LinkEditTaskProperty = DependencyProperty.Register(
            "LinkEditTask", typeof(TaskViewModel), typeof(PERTView), new PropertyMetadata(default(TaskViewModel)));

        public TaskViewModel LinkEditTask
        {
            get { return (TaskViewModel) GetValue(LinkEditTaskProperty); }
            set { SetValue(LinkEditTaskProperty, value); }
        }
        public bool IsLinkEditDisplayed
        {
            get { return (bool) GetValue(IsLinkEditDisplayedProperty); }
            set { SetValue(IsLinkEditDisplayedProperty, value); }
        }

        public string ToolTipText
        {
            get { return (string) GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        public Point CanvasMousePoint
        {
            get { return (Point) GetValue(CanvasMousePointProperty); }
            set { SetValue(CanvasMousePointProperty, value); }
        }

        public PertViewMode Mode
        {
            get { return (PertViewMode) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

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

        #endregion  

        public ICommand CloseRadialMenu => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.Normal;
            IsRadialMenuOpen = false;
            this.ToolTipText = "";
            this.LinkEditReset();
        });
        public ICommand ActivateNormalMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.Normal;
            this.IsRadialMenuOpen = false;
            this.LinkEditReset();
            this.ToolTipText = "";
        });
        public ICommand ActivateAddLinkMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.AddLink;
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to add link";
            this.LinkEditReset();
        });
        public ICommand ActivateRemoveLinkMode => new RelayCommand(() => 
        {
            this.Mode = PertViewMode.RemoveLink;
            this.ToolTipText = "Click to place new task";
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to remove link";
            this.LinkEditReset();

        });
        public ICommand ActivateAddTaskMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.AddTask;
            this.IsRadialMenuOpen = false;
            this.LinkEditReset();
            this.ToolTipText = "Click to place new task";
        });
        public ICommand ActivateRemoveTaskMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.RemoveTask;
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to remove task";
            this.LinkEditReset();
        });

        private double _minZoom = 0.1;
        private double _maxZoom = 3.0;
        private bool _isDragging;
        private Point _mouseDownPoint;
        private Point _taskDragStartPoint;
        private TaskViewModel _dragTask;

        public PERTView()
        {
            InitializeComponent();
            this.Zoom = 1;
            this.ToolTipText = "";
        }


        private void LinkEditReset()
        {
            // Because the link edit task uses the IsSelectedAncestor flag to display the red border on the diagram
            // wiping the 
            if (this.LinkEditTask != null)
            {
                if (!this.ViewModel.SelectedTask.Ancestors.Contains(this.LinkEditTask.Id))
                    this.LinkEditTask.IsSelectedAncestor = false;
            }
            this.LinkEditTask = null;
            this.IsLinkEditDisplayed = false;
            
            if (this.Mode == PertViewMode.AddLink)
                this.ToolTipText = "Click to select the prerequisite task to create a new link";

            if (this.Mode == PertViewMode.RemoveLink)
                this.ToolTipText = "Click to select the prerequisite task in the link to remove";
        }

        private void LinkEditClick(TaskViewModel task)
        {
            if (this.LinkEditTask == null)
            {
                // This is the first click
                this.LinkEditTask = task;
                this.IsLinkEditDisplayed = true;

                // Fake the mouseover 
                this.LinkEditTask.IsSelectedAncestor = true;

                if (this.Mode == PertViewMode.AddLink)
                    this.ToolTipText = "Click to select the dependant task to create a new link";

                if (this.Mode == PertViewMode.RemoveLink)
                    this.ToolTipText = "Click to select the dependant task in the link to remove";
            }
            else
            {
                this.ViewModel.SelectedTask = task;
                // This is the second click
                if (this.Mode == PertViewMode.AddLink)
                {
                    this.ViewModel.AddLink(this.LinkEditTask, task);
                    this.LinkEditReset();
                }
                else if (this.Mode == PertViewMode.RemoveLink)
                {
                    this.ViewModel.RemoveLink(this.LinkEditTask, task);
                    this.LinkEditReset();
                }
            }

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
            this.CanvasMousePoint = e.GetPosition(this.ViewCanvas);

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

        private void ViewAreaPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the mode is adding or removing a link, the click will be passed to the link edit click handling method
            // which will manage the semi-selected state and the transition between that and the creation of actual links
            if ((this.Mode == PertViewMode.AddLink || this.Mode == PertViewMode.RemoveLink) && this.ViewModel.IsMouseOverATask)
            {
                e.Handled = true;
                this.LinkEditClick(this.ViewModel.MouseOverTask);
                return;
            }

            // If the mode is removing a task, we pass to the task removal
            if (this.Mode == PertViewMode.RemoveTask && this.ViewModel.IsMouseOverATask)
            {
                e.Handled = true;
                this.ViewModel.RemoveTask(this.ViewModel.MouseOverTask);
                return;
            }

            // If the mode is adding a task, we create a task where the click happened
            if (this.Mode == PertViewMode.AddTask && e.LeftButton == MouseButtonState.Pressed)
            {
                this.ViewModel.AddTask(new TaskViewModel
                {
                    Name = "New Task/Stage",
                    Description = "- - -",
                    X = this.CanvasMousePoint.X,
                    Y = this.CanvasMousePoint.Y
                });
                return;
            }

            // If there there is no specified mode, we select the task and begin dragging
            if (e.LeftButton == MouseButtonState.Pressed && this.ViewModel.IsMouseOverATask)
            {
                this.ViewModel.SelectedTask = this.ViewModel.MouseOverTask;
                this._mouseDownPoint = e.GetPosition(ViewArea);
                this._taskDragStartPoint = this.ViewModel.MouseOverTask.CenterPoint;
                this._isDragging = true;
                this._dragTask = this.ViewModel.MouseOverTask;
            }
        }

        private void ViewAreaPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentShift = new Point
                {
                    X=e.GetPosition(ViewArea).X - _mouseDownPoint.X,
                    Y=e.GetPosition(ViewArea).Y - _mouseDownPoint.Y
                };
                this._dragTask.X = this._taskDragStartPoint.X + currentShift.X;
                this._dragTask.Y = this._taskDragStartPoint.Y + currentShift.Y;

            }
        }

        private void ViewAreaPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this._isDragging = false;
        }
    }
}
