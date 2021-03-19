using System.Collections.Generic;
using System.Windows.Input;
using WClipboard.Core.WPF.Native.Helpers;

namespace WClipboard.Core.WPF.Listeners
{
    public interface IGlobalKeyEventListener
    {
        void OnEvent(KeyboardHookEventArgs e, ModifierKeys modifierKeys);

        IEnumerable<Key> GetNotifyKeys();
    }
}
