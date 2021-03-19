using System.Collections.Generic;
using System.ComponentModel;

namespace WClipboard.Core.WPF.Native.Helpers
{
    public static class PropertiesDialogHelper
    {
        public static void OpenPropertiesDialog(string fullPath)
        {
            var info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = "\"" + fullPath + "\"";
            info.nShow = (int)NativeConsts.ShellShowCommands.SW_SHOW;
            info.fMask = (uint)NativeConsts.ShellExecuteMaskFlags.SEE_MASK_INVOKEIDLIST;
            if (!NativeMethods.ShellExecuteEx(ref info))
            {
                throw new Win32Exception();
            }
        }

        public static void OpenPropertiesDialog(IReadOnlyCollection<string> fullPaths)
        {
            //var dataObject = new ExtendedDataObject();
            //dataObject.SetFileDropList(fullPaths);
            //dataObject.SetShellFileIDList(fullPaths);
            //if (NativeMethods.SHMultiFileProperties(dataObject, 0) != 0)
            //{
            //    throw new Win32Exception();
            //}
        }
    }
}
