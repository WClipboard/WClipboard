using System;

namespace WClipboard.Core.Settings
{
    public interface IIOSetting : ISetting
    {
        Type Type { get; }
        object? Value { get; set; }
        object? GetDefaultValue(); 
    }
}
