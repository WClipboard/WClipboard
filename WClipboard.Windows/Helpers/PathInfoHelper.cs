using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using WClipboard.Windows.Extensions;
using WClipboard.Windows.Native;

namespace WClipboard.Windows.Helpers
{
    public enum IconType
    {
        Normal,
        Small,
        Large
    }

    public static class PathInfoHelper
    {
        public static BitmapSource? GetIcon(string fullPath, IconType iconType)
        {
            var info = new SHFILEINFO(true);

            var flags = NativeConsts.SHGFI.Icon;
            if (iconType == IconType.Large)
                flags |= NativeConsts.SHGFI.LargeIcon;
            else if (iconType == IconType.Small)
                flags |= NativeConsts.SHGFI.SmallIcon;

            var fileAttributes = NativeConsts.FILE_ATTRIBUTE.Normal;

            if (NativeMethods.SHGetFileInfo(fullPath, fileAttributes, out info, (uint)Marshal.SizeOf(info), flags) != 0 && info.hIcon != IntPtr.Zero)
            {
                return BitmapSourceConverters.ToBitmapSource(info.hIcon);
            }
            return null;
        }

        public static string? GetTypeName(string fullPath)
        {
            var info = new SHFILEINFO(true);

            var fileAttributes = NativeConsts.FILE_ATTRIBUTE.Normal;

            if (NativeMethods.SHGetFileInfo(fullPath, fileAttributes, out info, (uint)Marshal.SizeOf(info), NativeConsts.SHGFI.TypeName) != 0 && !string.IsNullOrWhiteSpace(info.szTypeName))
            {
                return info.szTypeName;
            }
            return null;
        }
    }
}
