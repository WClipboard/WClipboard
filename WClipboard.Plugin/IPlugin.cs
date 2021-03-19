using System;
using System.Collections.Generic;
using System.Text;
using WClipboard.Core.DI;

namespace WClipboard.Plugin
{
    public interface IPlugin : IStartup
    {
        string Name { get; }
    }
}
