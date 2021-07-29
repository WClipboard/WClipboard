using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WClipboard.Windows.Native;

namespace WClipboard.Windows
{
    public interface IGlobalKeyboardListener
    {
        event EventHandler<GlobalKeyboardEventArgs>? NotifyKeyStateChanged;

        HashSet<Key> NotifyKeys { get; }
    }

    public class GlobalKeyboardEventArgs : EventArgs
    {
        public Key NotifyKey { get; }
        public KeyStates State { get; }
        public IReadOnlyCollection<Key> DownStateKeys { get; }
        public long MessageTime { get; }

        internal GlobalKeyboardEventArgs(Key notifyKey, KeyStates state, IReadOnlyCollection<Key> downStateKeys, long messageTime)
        {
            NotifyKey = notifyKey;
            State = state;
            DownStateKeys = downStateKeys;
            MessageTime = messageTime;
        }
    }

    internal class GlobalKeyboardListener : IGlobalKeyboardListener, IDisposable
    {
        private IntPtr hhook;
        private HookCallback? cashedCallback;
        private readonly bool[] keyStates;

        public HashSet<Key> NotifyKeys { get; }

        public event EventHandler<GlobalKeyboardEventArgs>? NotifyKeyStateChanged;

        public GlobalKeyboardListener()
        {
            keyStates = new bool[256];

            NotifyKeys = new HashSet<Key>();

            cashedCallback = new HookCallback(HookProc);

            hhook = NativeMethods.SetWindowsHookEx((int)NativeConsts.HookId.WH_KEYBOARD_LL, cashedCallback, IntPtr.Zero, 0);
            if (hhook == IntPtr.Zero)
                throw new Win32Exception();
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProc(int code, int wParam, IntPtr lParam)
        {
            if (code < 0 || NotifyKeyStateChanged == null)
            {
                return NativeMethods.CallNextHookEx(hhook, code, wParam, lParam);
            }

            var castedLParam = Marshal.PtrToStructure<KeyboardDllHookStruct>(lParam);

            var state = wParam switch
            {
                (int)NativeConsts.Message.WM_KEYUP => KeyStates.Toggled,
                (int)NativeConsts.Message.WM_SYSKEYUP => KeyStates.Toggled,
                (int)NativeConsts.Message.WM_KEYDOWN => KeyStates.Down,
                (int)NativeConsts.Message.WM_SYSKEYDOWN => KeyStates.Down,
                _ => KeyStates.None,
            };

            if (castedLParam.vkCode < keyStates.Length)
            {
                keyStates[castedLParam.vkCode] = state == KeyStates.Down;
            }

            if (!NotifyKeys.Contains(KeyInterop.KeyFromVirtualKey(castedLParam.vkCode)))
            {
                return NativeMethods.CallNextHookEx(hhook, code, wParam, lParam);
            }

            var modifyStateKeys = new HashSet<Key>();
            for(int i = 0; i < keyStates.Length; i++)
            {
                if(keyStates[i]) {
                    modifyStateKeys.Add(KeyInterop.KeyFromVirtualKey(i));
                }
            }

            var key = KeyInterop.KeyFromVirtualKey(castedLParam.vkCode);
            long time = castedLParam.time;
            Task.Run(() => NotifyKeyStateChanged?.Invoke(this, new GlobalKeyboardEventArgs(key, state, modifyStateKeys, time)));

            return NativeMethods.CallNextHookEx(hhook, code, wParam, lParam);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                NativeMethods.UnhookWindowsHookEx(hhook);
                hhook = IntPtr.Zero;
                cashedCallback = null;

                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~GlobalKeyboardListener()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
