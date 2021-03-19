using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.CustomControls
{
    public class CustomWindowHeader : Control
    {
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(nameof(WindowTitle), typeof(string), typeof(CustomWindowHeader), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(CustomWindowHeader), new FrameworkPropertyMetadata(null));

        static CustomWindowHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindowHeader), new FrameworkPropertyMetadata(typeof(CustomWindowHeader)));
        }

        public string WindowTitle
        {
            get => (string)GetValue(WindowTitleProperty);
            set => SetValue(WindowTitleProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
