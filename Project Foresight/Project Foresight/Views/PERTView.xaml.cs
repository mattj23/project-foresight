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
using MathNet.Spatial.Euclidean;
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

        public static readonly DependencyProperty LinkSnippingStartPointProperty = DependencyProperty.Register(
            "LinkSnippingStartPoint", typeof(Point), typeof(PERTView), new PropertyMetadata(default(Point)));

        public static readonly DependencyProperty IsSnippingProperty = DependencyProperty.Register(
            "IsSnipping", typeof(bool), typeof(PERTView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty MarkersProperty = DependencyProperty.Register(
            "Markers", typeof(ObservableCollection<Point>), typeof(PERTView), new PropertyMetadata(default(ObservableCollection<Point>)));

        public static readonly DependencyProperty HightlightLinksProperty = DependencyProperty.Register(
            "HightlightLinks", typeof(ObservableCollection<LinkViewModel>), typeof(PERTView), new PropertyMetadata(default(ObservableCollection<LinkViewModel>)));

        public static readonly DependencyProperty IsInTaskAddSplitModeProperty = DependencyProperty.Register(
            "IsInTaskAddSplitMode", typeof(bool), typeof(PERTView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsResourcePaneVisibleProperty = DependencyProperty.Register(
            "IsResourcePaneVisible", typeof(bool), typeof(PERTView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ResourcePanePointProperty = DependencyProperty.Register(
            "ResourcePanePoint", typeof(Point), typeof(PERTView), new PropertyMetadata(default(Point)));

        public Point ResourcePanePoint
        {
            get { return (Point) GetValue(ResourcePanePointProperty); }
            set { SetValue(ResourcePanePointProperty, value); }
        }

        public bool IsResourcePaneVisible
        {
            get { return (bool) GetValue(IsResourcePaneVisibleProperty); }
            set { SetValue(IsResourcePaneVisibleProperty, value); }
        }
        public bool IsInTaskAddSplitMode
        {
            get { return (bool) GetValue(IsInTaskAddSplitModeProperty); }
            set { SetValue(IsInTaskAddSplitModeProperty, value); }
        }

        public ObservableCollection<LinkViewModel> HightlightLinks
        {
            get { return (ObservableCollection<LinkViewModel>) GetValue(HightlightLinksProperty); }
            set { SetValue(HightlightLinksProperty, value); }
        }

        public ObservableCollection<Point> Markers
        {
            get { return (ObservableCollection<Point>) GetValue(MarkersProperty); }
            set { SetValue(MarkersProperty, value); }
        }
        public bool IsSnipping
        {
            get { return (bool) GetValue(IsSnippingProperty); }
            set { SetValue(IsSnippingProperty, value); }
        }

        public Point LinkSnippingStartPoint
        {
            get { return (Point) GetValue(LinkSnippingStartPointProperty); }
            set { SetValue(LinkSnippingStartPointProperty, value); }
        }
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
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;
        });
        public ICommand ActivateNormalMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.Normal;
            this.IsRadialMenuOpen = false;
            this.LinkEditReset();
            this.ToolTipText = "";
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;
        });
        public ICommand ActivateAddLinkMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.AddLink;
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to add link";
            this.LinkEditReset();
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;
        });
        public ICommand ActivateRemoveLinkMode => new RelayCommand(() => 
        {
            this.Mode = PertViewMode.RemoveLink;
            this.ToolTipText = "Click to place new task";
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to remove link";
            this.LinkEditReset();
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;

        });
        public ICommand ActivateAddTaskMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.AddTask;
            this.IsRadialMenuOpen = false;
            this.LinkEditReset();
            this.ToolTipText = "Click to place new task";
            this.IsInTaskAddSplitMode = false;
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;
        });
        public ICommand ActivateRemoveTaskMode => new RelayCommand(() =>
        {
            this.Mode = PertViewMode.RemoveTask;
            this.IsRadialMenuOpen = false;
            this.ToolTipText = "Click to remove task";
            this.LinkEditReset();
            this.IsResourcePaneVisible = false;
            this._resourceEditTask = null;
        });

        private double _minZoom = 0.1;
        private double _maxZoom = 3.0;
        private Point _mouseDownPoint;
        private Point _taskDragStartPoint;
        private Dictionary<Guid, Point> _relativeDragPoints;
        private TaskViewModel _dragTask;
        private TaskViewModel _resourceEditTask;

        public PERTView()
        {
            InitializeComponent();
            this.Zoom = 1;
            this.ToolTipText = "";
            this._relativeDragPoints = new Dictionary<Guid, Point>();
            this.Markers = new ObservableCollection<Point>();
            this.HightlightLinks = new ObservableCollection<LinkViewModel>();
        }


        private void LinkEditReset()
        {
            // Because the link edit task uses the IsSelectedAncestor flag to display the red border on the diagram
            // wiping the 
            if (this.LinkEditTask != null && this.ViewModel.SelectedTask != null)
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

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.IsResourcePaneVisible = false;
                this._resourceEditTask = null;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.IsRadialMenuOpen = true;
                var mousePosition = e.GetPosition(this.ViewArea);
                this.RadialMargin = new Thickness(mousePosition.X - (this.RadialMenu.Width / 2.0), mousePosition.Y - (this.RadialMenu.Height / 2.0), 0, 0);
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
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this._dragStartShift = new Point(this.ShiftX, this.ShiftY);
                this._dragStartMouse = e.GetPosition(this.ViewArea);
                e.Handled = true;
            }


            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            // If the mode is adding or removing a link, the click will be passed to the link edit click handling method
            // which will manage the semi-selected state and the transition between that and the creation of actual links
            if ((this.Mode == PertViewMode.AddLink || this.Mode == PertViewMode.RemoveLink) && this.ViewModel.IsMouseOverATask)
            {
                e.Handled = true;
                this.LinkEditClick(this.ViewModel.MouseOverTask);
                return;
            }

            // If the mode is removing a link but the mouse is not over a task, we start the line drag deletion mode
            // for removing links, which at this point involves setting the snipping startpoint and the snipping flag
            if (this.Mode == PertViewMode.RemoveLink && !this.ViewModel.IsMouseOverATask)
            {
                this.IsSnipping = true;
                this.LinkSnippingStartPoint = e.GetPosition(ViewCanvas);
                this.ToolTipText = "Drag snipping line across links to break them";
            }

            // If the mode is removing a task, we pass to the task removal
            if (this.Mode == PertViewMode.RemoveTask && this.ViewModel.IsMouseOverATask)
            {
                e.Handled = true;
                this.ViewModel.RemoveTask(this.ViewModel.MouseOverTask);
                return;
            }

            if (this.Mode == PertViewMode.AddTask && this.ViewModel.IsMouseOverATask)
            {
                e.Handled = true;
                this.ViewModel.SplitTask(this.ViewModel.MouseOverTask);
            }
           

            // If there there is no specified mode, we select the task and begin dragging
            if (e.LeftButton == MouseButtonState.Pressed && this.ViewModel.IsMouseOverATask)
            {
                // Select the given task
                this.ViewModel.SelectedTask = this.ViewModel.MouseOverTask;

                // Prepare for dragging
                this._mouseDownPoint = e.GetPosition(ViewCanvas);
                this._dragTask = ViewModel.MouseOverTask;
                this._taskDragStartPoint = this._dragTask.CenterPoint;
                this._relativeDragPoints.Clear();
                foreach (var viewModelTask in this.ViewModel.Tasks)
                {
                    this._relativeDragPoints.Add(viewModelTask.Id, viewModelTask.CenterPoint);
                }

            }

        }

        private void ViewAreaPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragTask != null)
            {
                // Compute the shift
                var currentShift = new Point
                {
                    X=e.GetPosition(ViewCanvas).X - _mouseDownPoint.X,
                    Y=e.GetPosition(ViewCanvas).Y - _mouseDownPoint.Y
                };

                // Shift the drag task
                this.ShiftTask(this._dragTask.Id, currentShift);

                // Ancestors 
                if (Keyboard.IsKeyDown(Key.A))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        foreach (var dragTaskAncestor in _dragTask.AllAncestors)
                        {
                            this.ShiftTask(dragTaskAncestor, currentShift);
                        }
                    else
                        foreach (var dragTaskAncestor in _dragTask.Ancestors)
                        {
                            this.ShiftTask(dragTaskAncestor, currentShift);
                        }
                }
                if (Keyboard.IsKeyDown(Key.D))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                        foreach (var dragTaskDescendant in _dragTask.AllDescendants)
                        {
                            this.ShiftTask(dragTaskDescendant, currentShift);
                        }
                    else
                        foreach (var dragTaskDescendant in _dragTask.Descendants)
                        {
                            this.ShiftTask(dragTaskDescendant, currentShift);
                        }
                }

                this.ToolTipText =
                    "'A' to drag ancestors, 'D' to drag descendants, +Shift for all ancestors/descendants";

            }

            if (this.IsSnipping)
            {
                this.HightlightLinks.Clear();
                foreach (var linkViewModel in SnippingLinks(e.GetPosition(ViewCanvas)))
                {
                    this.HightlightLinks.Add(linkViewModel);
                }
                
            }

            if (this.Mode == PertViewMode.AddTask)
            {
                if (this.ViewModel.IsMouseOverATask)
                {
                    this.ToolTipText = "Click on this item to split it into two tasks";
                    this.IsInTaskAddSplitMode = true;
                }
                else
                {
                    this.ToolTipText = "Click to place new task";
                    this.IsInTaskAddSplitMode = false;
                }
            }

        }

        private List<LinkViewModel> SnippingLinks(Point mousePosition)
        {
            var links = new List<LinkViewModel>();
            Line2D snipLine;
            try
            {
                snipLine = new Line2D(new Point2D(LinkSnippingStartPoint.X, LinkSnippingStartPoint.Y),
                        new Point2D(mousePosition.X, mousePosition.Y));
            }
            catch (ArgumentException )
            {
                return links;
            }

            foreach (var link in this.ViewModel.Links)
            {
                Line2D linkLine;
                try
                {
                    linkLine = new Line2D(new Point2D(link.Start.X, link.Start.Y),
                    new Point2D(link.End.X, link.End.Y));
                }
                catch (ArgumentException)
                {
                    continue;
                }

                var intersection = snipLine.IntersectWith(linkLine);
                if (intersection == null)
                    continue;

                if (Math.Abs(linkLine.Length - (intersection.Value.DistanceTo(linkLine.StartPoint) + intersection.Value.DistanceTo(linkLine.EndPoint))) < 1.0 &&
                    Math.Abs(snipLine.Length - (intersection.Value.DistanceTo(snipLine.StartPoint) + intersection.Value.DistanceTo(snipLine.EndPoint))) < 1.0)
                    links.Add(link);
            }

            return links;
        }

        private void ShiftTask(Guid taskId, Point shift)
        {
            var shiftTask = this.ViewModel.GetTaskById(taskId);
            shiftTask.X = this._relativeDragPoints[taskId].X + shift.X;
            shiftTask.Y = this._relativeDragPoints[taskId].Y + shift.Y;
        }

        private void ViewAreaPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this._dragTask != null)
            {
                if (Math.Abs(this._taskDragStartPoint.X - this._dragTask.X) > 1.0 ||
                    Math.Abs(this._taskDragStartPoint.Y - this._dragTask.Y) > 1.0)
                {
                    this._dragTask.ReportJournalDataChanged();
                }

                this._dragTask = null;
                this.ToolTipText = "";
            }

            if (this.IsSnipping)
            {
                foreach (var link in SnippingLinks(e.GetPosition(ViewCanvas)))
                {
                    this.ViewModel.RemoveLink(link.Start, link.End);
                }
                this.HightlightLinks.Clear();
                this.IsSnipping = false;
                this.LinkEditReset();
            }
            
        }

        private void TaskView_OnResourceEditOnClick(object sender, TaskViewModel e)
        {
            this.IsResourcePaneVisible = true;
            this._resourceEditTask = e;
            this.ResourcePanePoint = CanvasMousePoint;
        }

        private void ResourceTaskOnClick(object sender, RoutedEventArgs e)
        {
            string resourceName = ((Button) sender).Tag as string;
            var resource = this.ViewModel.Organization.FindResourceByName(resourceName);
            if (resource != null)
            {
                if (!this._resourceEditTask.Resources.Contains(resource))
                    this._resourceEditTask.Resources.Add(resource);
            }

            this._resourceEditTask = null;
            this.IsResourcePaneVisible = false;
        }
    }
}
