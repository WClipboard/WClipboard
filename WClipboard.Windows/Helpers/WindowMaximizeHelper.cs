using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WClipboard.Windows.Native;

namespace WClipboard.Windows.Helpers
{
    public static class WindowMaximizeHelper
    {
        public static void AttachTo(Window window) => window.SourceInitialized += Window_SourceInitialized;

        private static void Window_SourceInitialized(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                var handle = (new WindowInteropHelper(window)).Handle;
                HwndSource.FromHwnd(handle).AddHook(WindowProc);
            }
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)NativeConsts.Message.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            var monitor = NativeMethods.MonitorFromWindow(hwnd, NativeConsts.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                if (!NativeMethods.GetMonitorInfo(monitor, monitorInfo))
                    throw new Win32Exception($"Cannot get monitor info for monitor {monitor}");
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}
