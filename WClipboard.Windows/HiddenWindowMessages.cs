// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2020 Philipp Sumi
// Contact and Information: http://www.hardcodet.net
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the Code Project Open License (CPOL);
// either version 1.0 of the License, or (at your option) any later
// version.
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE

// Based on WindowMessageSink.cs

using Hardcodet.Wpf.TaskbarNotification.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Interop;

namespace WClipboard.Windows
{

    /// <summary>
    /// WClipboard creates a hidden window that recieves messages. 
    /// Used in the ClipboardViewer Api and in the TaskbarIcon
    /// </summary>
    public interface IHiddenWindowMessages
    {
        /// <summary>
        /// Handle for the message window.
        /// </summary>
        IntPtr Handle { get; }

        event EventHandler? Disposed;

        void AddHook(HwndSourceHook hook);
        void RemoveHook(HwndSourceHook hook);
    }


    public class HiddenWindowMessages : IHiddenWindowMessages, IDisposable
    {
        /// <summary>
        /// A delegate that processes messages of the hidden
        /// native window that receives window messages. Storing
        /// this reference makes sure we don't loose our reference
        /// to the message window.
        /// </summary>
        private WindowProcedureHandler? messageHandler;

        /// <summary>
        /// Window class ID.
        /// </summary>
        internal string? WindowId { get; private set; }

        /// <summary>
        /// Handle for the message window.
        /// </summary>
        public IntPtr Handle { get; private set; }

        public event EventHandler? Disposed;

        private readonly List<HwndSourceHook> messageHooks;

        public HiddenWindowMessages()
        {
            messageHooks = new List<HwndSourceHook>();

            //generate a unique ID for the window
            WindowId = "WClipboard_" + Guid.NewGuid();

            //register window message handler
            messageHandler = OnWindowMessageReceived;

            // Create a simple window class which is reference through
            //the messageHandler delegate
            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = WindowId;

            // Register the window class
            WinApi.RegisterClass(ref wc);

            // Create the message window
            Handle = WinApi.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (Handle == IntPtr.Zero)
            {
                throw new Win32Exception("Message window handle was not a valid pointer");
            }
        }

        private IntPtr OnWindowMessageReceived(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            var handled = false;

            lock(messageHooks)
            {
                for(var i = 0; i < messageHooks.Count; i++)
                {
                    var count = messageHooks.Count;

                    var hook = messageHooks[i];

                    var ret = hook(hWnd, msg, wParam, lParam, ref handled);
                    if (handled)
                    {
                        return ret;
                    }

                    i -= count - messageHooks.Count;
                }
            }

            // Pass the message to the default window procedure
            return WinApi.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void AddHook(HwndSourceHook hook)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(HiddenWindowMessages));

            lock(messageHooks)
            {
                messageHooks.Insert(0, new HwndSourceHook(hook));
            }
        }

        public void RemoveHook(HwndSourceHook hook)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(HiddenWindowMessages));

            lock (messageHooks)
            {
                messageHooks.RemoveAll(otherHook => otherHook == hook);
            }
        }

        #region Dispose

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <remarks>This method is not virtual by design. Derived classes
        /// should override <see cref="Dispose(bool)"/>.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructor in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~HiddenWindowMessages()
        {
            Dispose(false);
        }

        /// <summary>
        /// Removes the windows hook that receives window
        /// messages and closes the underlying helper window.
        /// </summary>
        private void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (IsDisposed) return;
            IsDisposed = true;

            Disposed?.Invoke(this, new EventArgs());

            messageHooks.Clear();

            //always destroy the unmanaged handle (even if called from the GC)
            WinApi.DestroyWindow(Handle);
            messageHandler = null;
        }

        #endregion
    }
}
