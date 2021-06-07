using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Windows.Extensions;
using WClipboard.Windows.Native;

namespace WClipboard.Windows.Helpers
{
    public static class WindowInfoHelper
    {
        public static (WindowInfo, ProgramInfo)? GetClipboardOwnerWindowInfo()
        {
            var hWnd = NativeMethods.GetClipboardOwner();

            if (hWnd == IntPtr.Zero)
                return null;

            return GetWindowInfoForHandle(hWnd);
        }

        public static (WindowInfo, ProgramInfo)? GetForegroundWindowInfo()
        {
            var hWnd = NativeMethods.GetForegroundWindow();

            if (hWnd == IntPtr.Zero)
                return null;

            return GetWindowInfoForHandle(hWnd);
        }

        public static (WindowInfo, ProgramInfo)? GetFromWpfWindow(System.Windows.Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;

            if (handle == IntPtr.Zero)
                return null;

            NativeMethods.GetWindowThreadProcessId(handle, out var processId);

            return (new WindowInfo(window.Title, window.Icon), new ProgramInfo(processId));
        }

        public static (WindowInfo?, ProgramInfo)? GetForegroundOrClipboardOwnerInfo()
        {
            var forgroundInfo = ((WindowInfo?, ProgramInfo)?)GetForegroundWindowInfo();
            var clipboardOwnerInfo = GetClipboardOwnerWindowInfo()?.Item2;

            return (forgroundInfo, clipboardOwnerInfo) switch
            {
                (var fi, var coi) when fi.HasValue && !(coi is null) => fi.Value.Item2 == coi ? fi : (null, coi),
                (var fi, _) when fi.HasValue => fi,
                (_, var coi) when !(coi is null) => (null, coi),
                _ => null
            };
        }

        public static (WindowInfo, ProgramInfo)? GetWindowInfoForHandle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return null;

            NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);

            var process = Process.GetProcessById(processId);
            var iconSource = GetWindowIconSource(hWnd, Core.DI.DiContainer.SP!.GetRequiredService<IAppInfo>().ProcessId == process.Id);
            var title = GetWindowTitle(hWnd);

            return (new WindowInfo(title, iconSource), new ProgramInfo(processId));
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            var titleSize = NativeMethods.SendMessage(hWnd, (int)NativeConsts.Message.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();

            if (titleSize == 0)
                return string.Empty;

            var title = new StringBuilder(titleSize + 1);

            NativeMethods.SendMessage(hWnd, (int)NativeConsts.Message.WM_GETTEXT, title.Capacity, title);

            return title.ToString();
        }

        private static object? GetWindowIconSource(IntPtr hWnd, bool isIntern)
        {
            try
            {
                var iconHandle = default(IntPtr);
                iconHandle = NativeMethods.SendMessage(hWnd, (int)NativeConsts.Message.WM_GETICON, NativeConsts.ICON_SMALL2, IntPtr.Zero);

                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = NativeMethods.GetClassLongPtr(hWnd, NativeConsts.GCL_HICON);
                }

                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = NativeMethods.LoadIcon(IntPtr.Zero, (IntPtr)0x7F00/*IDI_APPLICATION*/);
                }

                if (iconHandle != IntPtr.Zero)
                {
                    return BitmapSourceConverters.ToBitmapSource(iconHandle, !isIntern);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
