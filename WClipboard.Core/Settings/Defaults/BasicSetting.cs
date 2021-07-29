using System;

namespace WClipboard.Core.Settings.Defaults
{
    public class BasicSetting<T> : IIOSetting
    {
        public Type Type { get; } = typeof(T);

        public object? Value { get; set; }

        public string Key { get; }

        private readonly Func<T> getDefaultValue;

        public object? GetDefaultValue()
        {
            return getDefaultValue();
        }

        public BasicSetting(string key, Func<T> getDefaultValue)
        {
            Key = key;
            this.getDefaultValue = getDefaultValue;
        }
    }
}
