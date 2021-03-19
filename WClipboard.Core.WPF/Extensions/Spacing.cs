using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.Extensions
{
    public static class Spacing
    {
        public static readonly DependencyProperty SpaceingProperty = DependencyProperty.RegisterAttached(nameof(Spacing), typeof(Thickness), typeof(Spacing), new FrameworkPropertyMetadata(new Thickness(0), OnSpacingChanged));

        public static void SetSpacing(Panel target, Thickness value)
        {
            target.SetValue(SpaceingProperty, value);
        }

        public static Thickness GetSpacing(Panel target)
        {
            return (Thickness)target.GetValue(SpaceingProperty);
        }

        private static void OnSpacingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(!(obj is Panel panel))
            {
                return;
            }

            if (panel.IsLoaded)
            {
                var oldValue = (Thickness)e.OldValue;
                var newValue = (Thickness)e.NewValue; 

                foreach(var child in panel.Children)
                {
                    if(child is FrameworkElement frameworkElement)
                    {
                        frameworkElement.Margin = frameworkElement.Margin.Sub(oldValue).Add(newValue);
                    }
                }
            }
            else
            {
                panel.Loaded += OnPanelLoaded;
            }
        }

        private static void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            if(!(sender is Panel panel))
            {
                return;
            }

            var spacing = GetSpacing(panel);

            foreach(var child in panel.Children.OfType<FrameworkElement>())
            {
                child.Margin = child.Margin.Add(spacing);
            }

            panel.Loaded -= OnPanelLoaded;
        }
    }
}
