using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Extensions
{
    public static class DragDropEx
    {
        //private static readonly Lazy<Cursor> dragCursor = new Lazy<Cursor>(() => DiContainer.SP!.GetRequiredService<ICursorManager>()["Drag"]);

        public static DependencyProperty DragProperty = DependencyProperty.RegisterAttached("Drag", typeof(IDragSource), typeof(DragDropEx), new PropertyMetadata(OnDragPropertyChanged));

        public static void SetDrag(UIElement target, IDragSource value)
        {
            target.SetValue(DragProperty, value);
        }

        public static IDragSource GetDrag(UIElement target)
        {
            return (IDragSource)target.GetValue(DragProperty);
        }

        private static void OnDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Thumb thumb)
            {
                ThumbEvents(thumb, e);
            }
            else if (d is UIElement uIElement)
            {
                UIElementEvents(uIElement, e);
            }
        }

        private static void UIElementEvents(UIElement target, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is null && !(e.NewValue is null))
            {
                target.MouseMove += Target_MouseMove;
                //target.GiveFeedback += Target_GiveFeedback;
            }
            else if (!(e.OldValue is null) && e.NewValue is null)
            {
                target.MouseMove -= Target_MouseMove;
                //target.GiveFeedback -= Target_GiveFeedback;
            }
        }

        //private static void Target_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        //{
        //    Mouse.SetCursor(dragCursor.Value);
        //    e.Handled = true;
        //}

        private static void DoDrag(UIElement uiElement)
        {
            var drag = GetDrag(uiElement);
            if (!(drag is null))
            {
                var data = drag.GetDragData(uiElement, out var effects);

                try
                {
                    DragDrop.DoDragDrop(uiElement, data, effects);
                }
                catch (COMException) { }
            }
        }

        private static void Target_MouseMove(object sender, MouseEventArgs e)
        {
            if(sender is UIElement uiElement && e.LeftButton == MouseButtonState.Pressed)
            {
                DoDrag(uiElement);
            }
        }

        private static void ThumbEvents(Thumb target, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is null && !(e.NewValue is null))
            {
                target.DragStarted += Target_DragStarted;
                //target.GiveFeedback += Target_GiveFeedback;
            }
            else if (!(e.OldValue is null) && e.NewValue is null)
            {
                target.DragStarted -= Target_DragStarted;
                //target.GiveFeedback -= Target_GiveFeedback;
            }
        }

        private static void Target_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                DoDrag(uiElement);
            }
        }
    }
}
