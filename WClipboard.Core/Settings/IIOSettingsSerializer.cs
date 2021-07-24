using System;
using System.Collections.Generic;
using System.Xml;

namespace WClipboard.Core.Settings
{
    public interface IIOSettingsSerializer
    {
        IEnumerable<Type> SupportedTypes { get; }
        void Serialize(object? value, XmlElement settingElement, IIOSettingsManager settingsManager);
        object? Deserialize(Type type, XmlElement settingElement, IIOSettingsManager settingsManager);
    }
}
