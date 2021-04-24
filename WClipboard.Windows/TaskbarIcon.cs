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

// Copied code parts of TaskbarIcon.cs and WindowMessageSink and modified it to incoperate it within my application

using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using System;
using System.Diagnostics;
using System.Windows.Input;
using WClipboard.Core;
using WClipboard.Windows.Native;

namespace WClipboard.Windows
{
    public class MouseActionEventArgs : EventArgs
    {
        public MouseAction MouseAction { get; }

        public MouseActionEventArgs(MouseAction mouseAction)
        {
            MouseAction = mouseAction;
        }
    }

    public interface ITaskbarIcon
    {
        event EventHandler<MouseActionEventArgs>? OnMouseAction;

        void Show();
        void Close();
    }

    public class TaskbarIcon : ITaskbarIcon, IDisposable
    {
        /// <summary>
        /// The ID of the message that is being received if the
        /// taskbar is (re)started.
        /// </summary>
        private readonly int taskbarRestartedMessageId; 

        private NotifyIconData iconData;
        private readonly object lockObject;

        private bool isTaskbarIconCreated;
        private bool isDoubleClick;

        public event EventHandler<MouseActionEventArgs>? OnMouseAction;

        public TaskbarIcon(IAppInfo appInfo, IHiddenWindowMessages messages)
        {
            lockObject = new object();

            taskbarRestartedMessageId = WinApi.RegisterWindowMessage("TaskbarCreated");

            messages.AddHook(WindowProc);
            messages.Disposed += Messages_Disposed;

            iconData = NotifyIconData.CreateDefault(messages.Handle);

            iconData.IconHandle = appInfo.Icon.Handle;

            Util.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Icon);

            // create the taskbar icon
            Show();
        }

        private void Messages_Disposed(object? sender, EventArgs e)
        {
            Dispose();
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == taskbarRestartedMessageId)
            {
                OnTaskbarCreated();
                handled = true;
            } 
            else if (msg == (int)NativeConsts.Message.WM_USER) // Callback message id
            {
                var taskbar_msg = lParam.ToInt32();

                switch (taskbar_msg)
                {
                    //case (int)NativeConsts.Message.WM_CONTEXTMENU:
                    //    // TODO: Handle WM_CONTEXTMENU, see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //    Debug.WriteLine("Unhandled WM_CONTEXTMENU");
                    //    break;

                    //case (int)NativeConsts.Message.WM_MOUSEMOVE:
                    //    MouseEventReceived?.Invoke(MouseEvent.MouseMove);
                    //    break;

                    //case (int)NativeConsts.Message.WM_LBUTTONDOWN:
                    //    OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.LeftClick));
                    //    break;

                    case (int)NativeConsts.Message.WM_LBUTTONUP:
                        if (!isDoubleClick)
                        {
                            OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.LeftClick));
                        }
                        isDoubleClick = false;
                        break;

                    case (int)NativeConsts.Message.WM_LBUTTONDBLCLK:
                        isDoubleClick = true;
                        OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.LeftDoubleClick));
                        break;

                    //case (int)NativeConsts.Message.WM_RBUTTONDOWN:
                    //    MouseEventReceived?.Invoke(MouseEvent.IconRightMouseDown);
                    //    break;

                    case (int)NativeConsts.Message.WM_RBUTTONUP:
                        if (!isDoubleClick)
                        {
                            OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.RightClick));
                        }
                        isDoubleClick = false;
                        break;

                    case (int)NativeConsts.Message.WM_RBUTTONDBLCLK:
                        isDoubleClick = true;
                        OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.RightDoubleClick));
                        break;

                    //case (int)NativeConsts.Message.WM_MBUTTONDOWN:
                    //    MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseDown);
                    //    break;

                    case (int)NativeConsts.Message.WM_MBUTTONUP:
                        if (!isDoubleClick)
                        {
                            OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.MiddleClick));
                        }
                        isDoubleClick = false;
                        break;

                    case (int)NativeConsts.Message.WM_MBUTTONDBLCLK:
                        isDoubleClick = true;
                        OnMouseAction?.Invoke(this, new MouseActionEventArgs(MouseAction.MiddleDoubleClick));
                        break;

                    //case (int)NativeConsts.Message.NIN_BALLOONSHOW:
                    //    BalloonToolTipChanged?.Invoke(true);
                    //    break;

                    //case (int)NativeConsts.Message.NIN_BALLOONHIDE:
                    //case (int)NativeConsts.Message.NIN_BALLOONTIMEOUT:
                    //    BalloonToolTipChanged?.Invoke(false);
                    //    break;

                    //case (int)NativeConsts.Message.NIN_BALLOONUSERCLICK:
                    //    MouseEventReceived?.Invoke(MouseEvent.BalloonToolTipClicked);
                    //    break;

                    //case (int)NativeConsts.Message.NIN_POPUPOPEN:
                    //    ChangeToolTipStateRequest?.Invoke(true);
                    //    break;

                    //case (int)NativeConsts.Message.NIN_POPUPCLOSE:
                    //    ChangeToolTipStateRequest?.Invoke(false);
                    //    break;

                    //case (int)NativeConsts.Message.NIN_SELECT:
                    //    // TODO: Handle NIN_SELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //    Debug.WriteLine("Unhandled NIN_SELECT");
                    //    break;

                    //case (int)NativeConsts.Message.NIN_KEYSELECT:
                    //    // TODO: Handle NIN_KEYSELECT see https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw
                    //    Debug.WriteLine("Unhandled NIN_KEYSELECT");
                    //    break;

                    //default:
                    //    Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                    //    break;
                }

                
                handled = true;
            }
            else if (msg == (int)NativeConsts.Message.WM_DPICHANGED)
            {
                SystemInfo.UpdateDpiFactors();
                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Recreates the taskbar icon if the whole taskbar was
        /// recreated (e.g. because Explorer was shut down).
        /// </summary>
        private void OnTaskbarCreated()
        {
            Close();
            Show();
        }

        /// <summary>
        /// Creates the taskbar icon. This message is invoked during initialization,
        /// if the taskbar is restarted, and whenever the icon is displayed.
        /// </summary>
        public void Show()
        {
            lock (lockObject)
            {
                if (isTaskbarIconCreated)
                {
                    return;
                }

                const IconDataMembers members = IconDataMembers.Message
                                                | IconDataMembers.Icon
                                                | IconDataMembers.Tip;

                //write initial configuration
                var status = Util.WriteIconData(ref iconData, NotifyCommand.Add, members);
                if (!status)
                {
                    // couldn't create the icon - we can assume this is because explorer is not running (yet!)
                    // -> try a bit later again rather than throwing an exception. Typically, if the windows
                    // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
                    // (we could also retry after a delay, but that's currently YAGNI)
                    return;
                }

                //set to most recent version
                SetVersion();
                //messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;

                isTaskbarIconCreated = true;
            }
        }

        /// <summary>
        /// Sets the version flag for the <see cref="iconData"/>.
        /// </summary>
        private void SetVersion()
        {
            iconData.VersionOrTimeout = (uint)NotifyIconVersion.Vista;
            bool status = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);

            if (!status)
            {
                Debug.Fail("Could not set version");
            }
        }

        /// <summary>
        /// Closes the taskbar icon if required.
        /// </summary>
        public void Close()
        {
            lock (lockObject)
            {
                // make sure we didn't schedule a creation

                if (!isTaskbarIconCreated)
                {
                    return;
                }

                Util.WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
                isTaskbarIconCreated = false;
            }
        }

        #region Dispose

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructor in types derived from this class.
        /// </para>
        /// </summary>
        ~TaskbarIcon()
        {
            Dispose(false);
        }


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
        /// Closes the tray and releases all resources.
        /// </summary>
        /// <summary>
        /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
        /// If disposing equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals <c>false</c>, the method
        /// has been called by the runtime from inside the finalizer and you
        /// should not reference other objects. Only unmanaged resources can
        /// be disposed.</param>
        /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
        /// the method has already been called.</remarks>
        private void Dispose(bool disposing)
        {
            // don't do anything if the component is already disposed
            if (IsDisposed || !disposing) return;

            lock (lockObject)
            {
                IsDisposed = true;

                // remove icon
                Close();
            }
        }

        #endregion
    }
}
