using System;

namespace WClipboard.Core.Settings.Defaults
{
    public class EnumSetting<T> : IIOSetting where T : Enum
    {
        public Type Type => typeof(T);

        public object? Value { get; set; }

        public string Key { get; }

        private readonly T defaultValue;
         
        public EnumSetting(string key, T defaultValue)
        {
            Key = key;
            this.defaultValue = defaultValue;
        }

        public object? GetDefaultValue()
        {
            return defaultValue;
        }
    }
}
