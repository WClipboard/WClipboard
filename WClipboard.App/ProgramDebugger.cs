using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WClipboard.App.Models;

namespace WClipboard.App
{
    internal class ProgramDebugger
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string? lpszWindow);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        private readonly Process? program;

        private ProgramDebugger(AppInfo appInfo)
        {
            ProcessStartInfo info = new ProcessStartInfo(appInfo.Path, "/nodebug")
            {
                RedirectStandardError = true,
                UseShellExecute = false
            };
            program = Process.Start(info);
        }

        internal static void Run(AppInfo appInfo)
        {
            var debugger = new ProgramDebugger(appInfo);
            debugger.Run();
        }

        private void Run()
        {
            if (program is null)
                return;

            program.WaitForExit();

            if(program.ExitCode != 0)
            {
                ShowError();
            }
        }

        private void ShowError()
        {
            var errorData = "Oops! WClipboard crashed :(\n\n ###### Error log: ######" + program?.StandardError.ReadToEnd();

            var notepad = Process.Start("notepad");
            notepad.WaitForInputIdle();
            var child = FindWindowEx(notepad.MainWindowHandle, IntPtr.Zero, "Edit", null);
            SendMessage(child, 0x00B1, 0, errorData);
            SendMessage(child, 0x00C2, 0, errorData);
        }
    }
}
