using System;
using System.Collections.Generic;
using System.Windows.Input;
using WClipboard.Core.WPF.Native.Helpers;

namespace WClipboard.Core.WPF.Listeners
{
    public class GlobalKeyDownEventListener : IGlobalKeyEventListener
    {
        public Key Key { get; }
        public ModifierKeys ModifierKeys { get; }
        public Action<GlobalKeyDownEventListener> Listener { get; }

        public GlobalKeyDownEventListener(Key key, ModifierKeys modifierKeys, Action<GlobalKeyDownEventListener> listener)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            Listener = listener;
        }

        void IGlobalKeyEventListener.OnEvent(KeyboardHookEventArgs e, ModifierKeys modifierKeys)
        {
            if (e.State == KeyStates.Down && modifierKeys == ModifierKeys && e.NotifyKey == Key)
                Listener(this);

        }

        IEnumerable<Key> IGlobalKeyEventListener.GetNotifyKeys()
        {
            yield return Key;
        }
    }
}
