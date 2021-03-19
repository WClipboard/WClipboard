using System;
using System.Collections.Generic;

namespace WClipboard.Core
{
    public interface IAppInfo
    {
        public string Name { get; }
        public string Path { get; }
        public int ProcessId { get; }
        public IReadOnlyList<string> Args { get; }

        Version Version { get; }
    }

    public static class IAppInfoExtensions
    {
        public static string GetDirectory(this IAppInfo appInfo)
        {
            return System.IO.Path.GetDirectoryName(appInfo.Path);
        }
    }
}
