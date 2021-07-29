using System;
using System.Collections.Generic;

namespace WClipboard.Core.Settings.Defaults
{
    public class ListSetting<T> : IIOSetting
    {
        public Type Type => typeof(List<T>);

        object? IIOSetting.Value { get => Value; set => Value = (List<T>)value!; }
        public List<T> Value { get; set; }

        public string Key { get; }

        private readonly List<T> defaultValue;
        public object? GetDefaultValue() => defaultValue;

        public ListSetting(string key, List<T> defaultValue)
        {
            Key = key;
            Value = defaultValue;
            this.defaultValue = defaultValue;
        }

        public ListSetting(string key) : this(key, new List<T>())
        { }
    }
}
