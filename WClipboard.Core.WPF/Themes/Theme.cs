using System;
using System.Reflection;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Themes
{
    public class Theme
    {
        public string Name { get; }
        public string ResourceDictionaryLocation { get; }
        public string Source { get; }

        public Theme(string name, string resourceDictionaryLocation, string? source = null, string? assemblyName = null)
        {
            Name = name;
            assemblyName ??= Assembly.GetCallingAssembly().GetName().Name ?? throw new ArgumentException($"{nameof(assemblyName)} is not set and cannot retrieve the {nameof(AssemblyName.Name)} of {nameof(Assembly.GetCallingAssembly)}");
            ResourceDictionaryLocation = ResourceDictionaryUtilities.CheckLocation(resourceDictionaryLocation, assemblyName);
            Source = source ?? assemblyName[(assemblyName.LastIndexOf('.') + 1)..];
        }
    }
}
