using System;
using System.Collections.Generic;
using System.Xml;

namespace WClipboard.Core.Settings.Defaults
{
    public class EnumIOSettingsSerializer : IIOSettingsSerializer
    {
        public IEnumerable<Type> SupportedTypes => new[] { typeof(Enum) };

        public object? Deserialize(Type type, XmlElement settingNode, IIOSettingsManager settingsManager)
        {
            return Enum.Parse(type, settingNode.InnerText);
        }

        public void Serialize(object? value, XmlElement settingNode, IIOSettingsManager settingsManager)
        {
            settingNode.InnerText = Convert.ToString(value) ?? string.Empty;
        }
    }
}
