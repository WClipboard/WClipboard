using System;
using System.Runtime.InteropServices;

namespace WClipboard.Core.WPF.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        /// <summary>x coordinate of point.</summary>
        public int x;
        /// <summary>y coordinate of point.</summary>
        public int y;
        /// <summary>Construct a point of coordinates (x,y).</summary>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //https://docs.microsoft.com/nl-nl/windows/desktop/api/winuser/ns-winuser-tagmonitorinfo
    internal class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    //https://msdn.microsoft.com/nl-nl/9439cb6c-f2f7-4c27-b1d7-8ddf16d81fe8
    internal struct RECT : IEquatable<RECT>
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public static readonly RECT Empty = new RECT();

        public int Width => Math.Abs(right - left);
        public int Height => Math.Abs(bottom - top);
        public bool IsEmpty => left >= right || top >= bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }

        public override string ToString()
        {
            if (this == Empty) { return "RECT [Empty]"; }
            return $"RECT [ left : {left} / top : {top} / right : {right} / bottom : {bottom} ]";
        }

        public override bool Equals(object? obj)
        {
            if (obj is RECT other)
                return Equals(other);
            else
                return false;
        }
        /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        public bool Equals(RECT other) => this == other;

        /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        public static bool operator ==(RECT rect1, RECT rect2)
        {
            return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
        }

        /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        public static bool operator !=(RECT rect1, RECT rect2)
        {
            return !(rect1 == rect2);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //https://docs.microsoft.com/nl-nl/windows/desktop/api/shellapi/ns-shellapi-shfileinfoa
    internal struct SHFILEINFO
    {
        public SHFILEINFO(bool b)
        {
            hIcon = IntPtr.Zero;
            iIcon = 0;
            dwAttributes = 0;
            szDisplayName = "";
            szTypeName = "";
        }
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeConsts.MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeConsts.MAX_TYPE)]
        public string szTypeName;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //https://docs.microsoft.com/en-us/windows/desktop/api/shellapi/ns-shellapi-_shellexecuteinfoa
    internal struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    //https://docs.microsoft.com/nl-nl/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)
    internal delegate int HookCallback(int code, int wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    //https://docs.microsoft.com/nl-nl/windows/win32/api/winuser/ns-winuser-kbdllhookstruct?redirectedfrom=MSDN
    internal struct KeyboardDllHookStruct
    {
        internal int vkCode;
        internal int scanCode;
        internal int flags;
        internal int time;
        internal IntPtr dwExtraInfo;
    }
}
