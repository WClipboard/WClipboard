using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.CustomControls
{
    public class Icon : Control
    {
        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }
    }
}
