using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.Extensions
{
    public static class Spacing
    {
        public static readonly DependencyProperty SpaceingProperty = DependencyProperty.RegisterAttached(nameof(Spacing), typeof(Thickness), typeof(Spacing), new FrameworkPropertyMetadata(new Thickness(0), OnSpacingChanged));
        public static readonly DependencyProperty AddProperty = DependencyProperty.RegisterAttached("Add", typeof(Thickness), typeof(Spacing), new FrameworkPropertyMetadata(new Thickness(0), OnAddChanged));
        public static readonly DependencyProperty OverwriteProperty = DependencyProperty.RegisterAttached("Overwrite", typeof(Thickness?), typeof(Spacing), new FrameworkPropertyMetadata(null, OnOverwriteChanged));

        public static void SetSpacing(Panel target, Thickness value)
        {
            target.SetValue(SpaceingProperty, value);
        }

        public static Thickness GetSpacing(Panel target)
        {
            return (Thickness)target.GetValue(SpaceingProperty);
        }

        public static void SetAdd(FrameworkElement target, Thickness value)
        {
            target.SetValue(AddProperty, value);
        }

        public static Thickness GetAdd(FrameworkElement target)
        {
            return (Thickness)target.GetValue(AddProperty);
        }

        public static void SetOverwrite(FrameworkElement target, Thickness? value)
        {
            target.SetValue(OverwriteProperty, value);
        }

        public static Thickness? GetOverwrite(FrameworkElement target)
        {
            return (Thickness?)target.GetValue(OverwriteProperty);
        }

        private static void OnSpacingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(!(obj is Panel panel))
            {
                return;
            }

            if (panel.IsLoaded)
            {
                UpdatePanel(panel, (Thickness)e.NewValue);
            }
            else
            {
                panel.Loaded += OnPanelLoaded;
            }
        }

        private static void OnAddChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameworkElement fe)
            {
                var oldValue = (Thickness)e.OldValue;
                var newValue = (Thickness)e.NewValue;

                fe.Margin = fe.Margin.Sub(oldValue).Add(newValue);
            }
        }

        private static void OnOverwriteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameworkElement fe)
            {
                var oldValue = (Thickness)e.OldValue;
                var newValue = (Thickness)e.NewValue;

                fe.Margin = fe.Margin.Sub(oldValue).Add(newValue);
            }
        }

        private static void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            if(!(sender is Panel panel))
            {
                return;
            }

            UpdatePanel(panel, GetSpacing(panel));

            panel.Loaded -= OnPanelLoaded;
        }

        private static void UpdatePanel(Panel panel, Thickness spacing)
        {
            foreach (var child in panel.Children.OfType<FrameworkElement>())
            {
                child.Margin = GetOverwrite(child) ?? spacing.Add(GetAdd(child));
            }
        }
    }
}
