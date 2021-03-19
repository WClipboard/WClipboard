using System;

namespace WClipboard.Core.Settings.Defaults
{
    public class StringSetting : IIOSetting
    {
        public Type Type { get; } = typeof(string);

        public object? Value { get; set; }

        public string Key { get; }

        private readonly Func<string?> getDefaultValue;

        public object? GetDefaultValue()
        {
            return getDefaultValue();
        }

        public StringSetting(string key, Func<string?> getDefaultValue)
        {
            Key = key;
            this.getDefaultValue = getDefaultValue;
        }
    }
}
