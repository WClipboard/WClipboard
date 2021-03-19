using System.Collections.Generic;
using System.Diagnostics;
using WClipboard.Core.Extensions;
using WClipboard.Core;
using System.Reflection;
using System;

namespace WClipboard.App.Models
{
    internal class AppInfo : IAppInfo
    {
        public string Name { get; }

        public string Path { get; }
        public int ProcessId { get; }

        public IReadOnlyList<string> Args { get; }

        public Version Version { get; }

        internal AppInfo(IReadOnlyList<string> args)
        {
            Args = args;

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();

            Name = assemblyName.Name;
            Version = assemblyName.Version;

            using(var curProccess = Process.GetCurrentProcess())
            {
                Path = curProccess.GetPath();
                ProcessId = curProccess.Id;
            }
        }
    }
}
