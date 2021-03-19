using System;
using System.Collections.Generic;

namespace WClipboard.Core.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return (T)value;
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T Parse<T>(string name) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static string GetName<T>(T value) where T : Enum
        {
            return Enum.GetName(typeof(T), value);
        }

        public static string[] GetNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }
    }
}
