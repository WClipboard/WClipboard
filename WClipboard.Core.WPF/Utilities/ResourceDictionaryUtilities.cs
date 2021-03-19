using System;
using System.Reflection;
using System.Windows;

namespace WClipboard.Core.WPF.Utilities
{
    public static class ResourceDictionaryUtilities
    {
        public static ResourceDictionary Get(string location)
        {
            return new ResourceDictionary()
            {
                Source = GetSourceUri(location)
            };
        }

        public static ResourceDictionary Get(string location, Assembly assembly)
        {
            return Get(location, assembly.GetName().FullName);
        }

        public static ResourceDictionary Get(string location, string assemblyName)
        {
            return Get(CheckLocation(location, assemblyName));
        }

        public static Uri GetSourceUri(string location)
        {
            return new Uri(location, UriKind.RelativeOrAbsolute);
        }

        public static string CheckLocation(string location, string? assemblyName)
        {
            if (!location.Contains(';'))
            {
                if (!location.StartsWith("component/"))
                {
                    location = "component/" + location;
                }
                location = (assemblyName ?? throw new ArgumentNullException(nameof(assemblyName), $"With this specific location the {nameof(assemblyName)} must be provided")) + ";" + location;
            }
            return location;
        }
    }
}
