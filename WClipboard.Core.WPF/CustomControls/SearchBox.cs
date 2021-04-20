using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WClipboard.Core.WPF.CustomControls
{
    public class SearchBox : ComboBox
    {
        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));

            IsEditableProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(true));
            IsReadOnlyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(false));
            ShouldPreserveUserEnteredPrefixProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(true));
            StaysOpenOnEditProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(true));
            IsTabStopProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(false));

            IsDropDownOpenProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(OnIsDropDownOpenChanged));
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool newValue && newValue == false)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
