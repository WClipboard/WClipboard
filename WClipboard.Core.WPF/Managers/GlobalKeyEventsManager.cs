using System;
using System.Collections.Generic;
using System.Windows.Input;
using WClipboard.Core.WPF.Native.Helpers;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Listeners;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Managers
{
    public interface IGlobalKeyEventsManager
    {
        void AddListener(IGlobalKeyEventListener listener);
        void RemoveListener(IGlobalKeyEventListener listener);
    }

    public sealed class GlobalKeyEventsManager : IDisposable, IGlobalKeyEventsManager
    {
        private static readonly Key[] defaultModifierKeys = new Key[] { Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LeftAlt, Key.RightAlt, Key.LWin, Key.RWin };

        private readonly KeyHookHelper keyboardHookHelper;

        private readonly List<IGlobalKeyEventListener> globalKeyListeners;

        public GlobalKeyEventsManager()
        {
            keyboardHookHelper = new KeyHookHelper();
            keyboardHookHelper.NotifyKeyStateChanged += KeyboardHookHelper_NotifyKeyStateChanged;
            keyboardHookHelper.NotifyKeys.AddRange(defaultModifierKeys);

            globalKeyListeners = new List<IGlobalKeyEventListener>();
        }

        private void KeyboardHookHelper_NotifyKeyStateChanged(object? sender, KeyboardHookEventArgs e)
        {
            var modifierKeys = e.DownStateKeys.GetModifierKeys();

            foreach (var listener in globalKeyListeners)
            {
                listener.OnEvent(e, modifierKeys);
            }
        }

        public void AddListener(IGlobalKeyEventListener listener)
        {
            globalKeyListeners.Add(listener);
            keyboardHookHelper.NotifyKeys.AddRange(listener.GetNotifyKeys());
        }

        public void RemoveListener(IGlobalKeyEventListener listener)
        {
            globalKeyListeners.Remove(listener);
            var notifyKeysToRemove = new List<Key>(listener.GetNotifyKeys());
            notifyKeysToRemove.RemoveRange(defaultModifierKeys);
            foreach (var otherListener in globalKeyListeners)
            {
                notifyKeysToRemove.RemoveRange(otherListener.GetNotifyKeys());
            }
            keyboardHookHelper.NotifyKeys.RemoveRange(notifyKeysToRemove);
        }

        public void Dispose() => ((IDisposable)keyboardHookHelper).Dispose();
    }
}
