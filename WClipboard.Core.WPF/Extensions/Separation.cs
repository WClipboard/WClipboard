using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.Extensions
{
    public static class Separation
    {
        public static readonly DependencyProperty SeparatorTemplateProperty = DependencyProperty.RegisterAttached("SeparatorTemplate", typeof(ControlTemplate), typeof(Separation), new FrameworkPropertyMetadata(OnSeparatorTemplateChanged));
        private static readonly DependencyProperty IsSeparatorProperty = DependencyProperty.RegisterAttached("IsSeparator", typeof(bool), typeof(Separation), new FrameworkPropertyMetadata(false));

        public static void SetSeparatorTemplate(Panel target, ControlTemplate value)
        {
            target.SetValue(SeparatorTemplateProperty, value);
        }

        public static ControlTemplate GetSeparatorTemplate(Panel target)
        {
            return (ControlTemplate)target.GetValue(SeparatorTemplateProperty);
        }

        private static void SetIsSeparator(DependencyObject target, bool value)
        {
            if(value)
            {
                target.SetValue(IsSeparatorProperty, value);
            }
            else
            {
                target.ClearValue(IsSeparatorProperty);
            }
        }

        private static bool GetIsSeparator(DependencyObject target)
        {
            return (bool)target.GetValue(IsSeparatorProperty);
        }

        private static void UpdatePanel(Panel target)
        {
            var SeparatorTemplate = GetSeparatorTemplate(target);

            if(SeparatorTemplate == null)
            {
                for(int i = 0; i < target.Children.Count; i++)
                {
                    var child = target.Children[i];
                    if(GetIsSeparator(child))
                    {
                        target.Children.RemoveAt(i);
                        i--;
                    }
                }
            }

            for (int i = 0; i < target.Children.Count; i++)
            {
                var child = target.Children[i];

                bool shouldBeSeparator = i % 2 == 1;
                bool isSeparator = GetIsSeparator(child);

                if (shouldBeSeparator && !isSeparator)
                {
                    var Separator = new Control() { Template = SeparatorTemplate };
                    SetIsSeparator(Separator, true);
                    target.Children.Insert(i, Separator);
                }
                else if (!shouldBeSeparator && isSeparator)
                {
                    target.Children.RemoveAt(i);
                    i--;
                }
                else if (isSeparator && child is Control Separator)
                {
                    Separator.Template = SeparatorTemplate;
                }
            }
        }

        private static void OnSeparatorTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Panel panel))
            {
                return;
            }

            if (panel.IsLoaded)
            {
                UpdatePanel(panel);
            }
            else
            {
                panel.Loaded += OnPanelLoaded;
            }
        }

        private static void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is Panel panel))
            {
                return;
            }

            UpdatePanel(panel);

            panel.Loaded -= OnPanelLoaded;
        }
    }
}
