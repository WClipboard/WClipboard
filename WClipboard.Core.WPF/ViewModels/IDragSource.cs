using System.Windows;

namespace WClipboard.Core.WPF.ViewModels
{
    public interface IDragSource
    {
        object GetDragData(DependencyObject dragSource, out DragDropEffects effects);
    }
}
