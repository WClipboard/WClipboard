using System;
using System.Collections.Generic;
using System.Drawing;

namespace WClipboard.Core
{
    public interface IAppInfo
    {
        public string Name { get; }
        public string Path { get; }
        public int ProcessId { get; }
        public IReadOnlyList<string> Args { get; }

        Version Version { get; }

        Icon Icon { get; }
    }

    public static class IAppInfoExtensions
    {
        public static string GetDirectory(this IAppInfo appInfo)
        {
            return System.IO.Path.GetDirectoryName(appInfo.Path)!; //Is not null and is no root directory
        }
    }
}
