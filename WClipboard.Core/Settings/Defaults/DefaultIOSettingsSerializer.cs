using System;
using System.Collections.Generic;
using System.Xml;

namespace WClipboard.Core.Settings.Defaults
{
    public class DefaultIOSettingsSerializer : IIOSettingsSerializer
    {
        public IEnumerable<Type> SupportedTypes { get; } = new [] { typeof(IConvertible) };

        public object? Deserialize(Type type, XmlElement settingNode, IIOSettingsManager settingsManager)
        {
            return Convert.ChangeType(settingNode.InnerText, type);
        }
        public void Serialize(object? value, XmlElement settingNode, IIOSettingsManager settingsManager)
        {
            settingNode.InnerText = Convert.ToString(value) ?? string.Empty;
        }
    }
}
