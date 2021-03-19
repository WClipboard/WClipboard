using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.CustomControls
{
    public class ContaineredItemsControl : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentControl();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Even wrap other ContentControls
            return false;
        }
    }
}
