﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using WClipboard.Windows;

namespace WClipboard.Core.WPF.Listeners
{
    public class GlobalKeyUpEventListener : IGlobalKeyEventListener
    {
        public Key Key { get; }
        public ModifierKeys ModifierKeys { get; }
        public Action<GlobalKeyUpEventListener, long> Listener { get; }
        private long? lastDownTime;

        public GlobalKeyUpEventListener(Key key, ModifierKeys modifierKeys, Action<GlobalKeyUpEventListener, long> listener)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            Listener = listener;
            lastDownTime = null;
        }

        void IGlobalKeyEventListener.OnEvent(GlobalKeyboardEventArgs e, ModifierKeys modifierKeys)
        {
            if (e.State == KeyStates.Down && modifierKeys == ModifierKeys && e.NotifyKey == Key && !lastDownTime.HasValue)
            {
                lastDownTime = e.MessageTime;
            }
            else if (e.State == KeyStates.Toggled && e.NotifyKey == Key && lastDownTime.HasValue)
            {
                Listener(this, e.MessageTime - lastDownTime.Value);
                lastDownTime = null;
            }
        }

        IEnumerable<Key> IGlobalKeyEventListener.GetNotifyKeys()
        {
            yield return Key;
        }
    }
}
