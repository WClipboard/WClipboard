using System.Collections.Generic;
using System.Windows.Input;

namespace WClipboard.Core.WPF.Extensions
{
    public static class KeyExtensions
    {
        public static ModifierKeys GetModifierKeys(this IEnumerable<Key> keys)
        {
            var modifierKeys = ModifierKeys.None;

            foreach(var key in keys)
            {
                if (key == Key.LeftCtrl || key == Key.RightCtrl)
                    modifierKeys |= ModifierKeys.Control;
                else if (key == Key.LeftShift || key == Key.RightShift)
                    modifierKeys |= ModifierKeys.Shift;
                else if (key == Key.LeftAlt || key == Key.RightAlt)
                    modifierKeys |= ModifierKeys.Alt;
                else if (key == Key.LWin || key == Key.RWin)
                    modifierKeys |= ModifierKeys.Windows;
            }

            return modifierKeys;
        }

        public static ModifierKeys GetModifierKey(this Key key)
        {
            switch (key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return ModifierKeys.Control;
                case Key.LeftShift:
                case Key.RightShift:
                    return ModifierKeys.Shift;
                case Key.LeftAlt:
                case Key.RightAlt:
                    return ModifierKeys.Alt;
                case Key.LWin:
                case Key.RWin:
                    return ModifierKeys.Windows;
                default:
                    return ModifierKeys.None;
            }
        }
    }
}
