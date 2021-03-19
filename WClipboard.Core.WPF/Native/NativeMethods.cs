using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace WClipboard.Core.WPF.Native
{
    internal static class NativeMethods
    {
        #region user32.dll
        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getmonitorinfoa
        //Failure: false
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-monitorfromwindow
        internal static extern IntPtr MonitorFromWindow(IntPtr hWnd, int flags);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendmessage
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendmessage
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-loadicona
        //Failure: zero
        internal static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        private static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLong64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 64 bit version maybe loses significant 64-bit specific information
        /// </summary>
        // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getclasslongptra
        // Failure: zero
        internal static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return new IntPtr(GetClassLong32(hWnd, nIndex));
            else
                return GetClassLong64(hWnd, nIndex);
        }

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getwindowthreadprocessid
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getforegroundwindow
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-destroyicon
        //Failure: false
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setclipboardviewer
        internal static extern IntPtr SetClipboardViewer(IntPtr hWnd);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-changeclipboardchain
        internal static extern bool ChangeClipboardChain(
            IntPtr hWndRemove, // handle to window to remove
            IntPtr hWndNewNext // handle to next window
        );

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeystate
        internal static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        //https://docs.microsoft.com/nl-nl/windows/win32/api/winuser/nf-winuser-getkeyboardstate
        private static extern bool GetKeyboardStateP(byte[] lpKeyState);

        internal static bool GetKeyboardState(out byte[] lpKeyState)
        {
            lpKeyState = new byte[256];
            return GetKeyboardStateP(lpKeyState);
        }

        [DllImport("user32.dll", SetLastError = true)]
        //https://docs.microsoft.com/nl-nl/windows/win32/api/winuser/nf-winuser-setwindowshookexw
        //Failure: Zero
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookCallback callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/nl-nl/windows/win32/api/winuser/nf-winuser-unhookwindowshookex
        //Failure: Zero / False
        internal static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        //https://docs.microsoft.com/nl-nl/windows/win32/api/winuser/nf-winuser-callnexthookex
        internal static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);
        #endregion

        #region shell32.dll
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/shellapi/nf-shellapi-shgetfileinfoa
        // If uFlags does not contain EXETYPE or SYSICONINDEX, the return value is nonzero if successful, or zero otherwise.
        // If uFlags contains the EXETYPE flag, the return value specifies the type of the executable file. It will be one of the following values.
        internal static extern int SHGetFileInfo(
            string pszPath,
            NativeConsts.FILE_ATTRIBUTE dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbfileInfo,
            NativeConsts.SHGFI uFlags
        );

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //https://docs.microsoft.com/nl-nl/windows/desktop/api/shellapi/nf-shellapi-shellexecuteexa
        internal static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //https://docs.microsoft.com/nl-nl/windows/desktop/api/shlobj/nf-shlobj-shmultifileproperties
        //Failure: != 0
        internal static extern int SHMultiFileProperties(IDataObject dataObj, int flags);


        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/shlobj_core/nf-shlobj_core-ilcreatefrompath
        //Use ILFree to free resource
        internal static extern IntPtr ILCreateFromPath(string path);

        [DllImport("shell32.dll", CharSet = CharSet.None)]
        //https://docs.microsoft.com/nl-nl/windows/desktop/api/shlobj_core/nf-shlobj_core-ilfree
        internal static extern void ILFree(IntPtr pidl);

        [DllImport("shell32.dll", CharSet = CharSet.None)]
        //https://docs.microsoft.com/en-us/windows/desktop/api/shlobj_core/nf-shlobj_core-ilgetsize
        internal static extern uint ILGetSize(IntPtr pidl);

        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KnownFolderID.
        /// </summary>
        /// <param name="rfid">A KnownFolderID that identifies the folder.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be
        ///     0; otherwise, one or more of the KnownFolderFlag values.</param>
        /// <param name="hToken">An access token that represents a particular user. If this
        ///     parameter is NULL, which is the most common usage, the function requests the known
        ///     folder for the current user. Assigning a value of -1 indicates the Default User.
        ///     The default user profile is duplicated when any new user account is created.
        ///     Note that access to the Default User folders requires administrator privileges.
        ///     </param>
        /// <param name="ppszPath">When this method returns, contains the address of a string that
        ///     specifies the path of the known folder. The returned path does not include a
        ///     trailing backslash.</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
        [DllImport("shell32.dll")]
        internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
        #endregion
    }
}
