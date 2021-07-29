using System.Collections.Generic;
using System.Windows.Input;
using WClipboard.Windows;

namespace WClipboard.Core.WPF.Listeners
{
    public interface IGlobalKeyEventListener
    {
        void OnEvent(GlobalKeyboardEventArgs e, ModifierKeys modifierKeys);

        IEnumerable<Key> GetNotifyKeys();
    }
}
