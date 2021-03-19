using System;
using System.Collections.Generic;
using System.Xml;

namespace WClipboard.Core.Settings.Defaults
{
    public class DefaultIOSettingsSerializer : IIOSettingsSerializer
    {
        public IEnumerable<Type> SupportedTypes { get; } = new [] { typeof(IConvertible) };

        public object? Deserialize(Type type, XmlElement settingNode)
        {
            return Convert.ChangeType(settingNode.InnerText, type);
        }
        public void Serialize(object? value, XmlElement settingNode)
        {
            settingNode.InnerText = Convert.ToString(value);
        }
    }
}
