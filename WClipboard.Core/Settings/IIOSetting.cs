using System;

namespace WClipboard.Core.Settings
{
    public interface IIOSetting : ISetting
    {
        Type Type { get; }
        object? Value { get; set; }
        object? GetDefaultValue(); 
    }

    public static class IIOSettingExtensions
    {
        public static T GetValue<T>(this IIOSetting setting)
        {
            return (T)setting.Value!;
        }
    }
}
