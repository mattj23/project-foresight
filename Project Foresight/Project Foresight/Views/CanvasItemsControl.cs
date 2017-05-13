using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Project_Foresight.Views
{
    public class CanvasItemsControl : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(
                            DependencyObject element,
                            object item)
        {
            Binding leftBinding = new Binding() { Path = new PropertyPath("X") , Mode = BindingMode.TwoWay};
            Binding topBinding = new Binding() { Path = new PropertyPath("Y"), Mode = BindingMode.TwoWay };

            FrameworkElement contentControl = (FrameworkElement)element;
            contentControl.SetBinding(Canvas.LeftProperty, leftBinding);
            contentControl.SetBinding(Canvas.TopProperty, topBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}