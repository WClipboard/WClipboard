using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using WClipboard.Core.LifeCycle;
using WClipboard.Core.Extensions.Xml;
using WClipboard.Core.IO;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.Settings
{
    public interface IIOSettingsManager
    {
        void AddSettings(IEnumerable<IIOSetting> settings);
        void AddSettings(params IIOSetting[] settings);
        IEnumerable<IIOSetting> GetSection(string key);
        IIOSetting GetSetting(string key);
        IEnumerable<IIOSetting> GetSettings();
        object? GetValue(string key);
        void AddSerializers(IEnumerable<IIOSettingsSerializer> serializers);
        void AddSerializers(params IIOSettingsSerializer[] serializers);
        void Save();
        IIOSettingsSerializer FindSerializer(Type type);
    }

    public class IOSettingsManager : IIOSettingsManager, IAfterDIContainerBuildListener
    {
        private const string FILE_NAME = @"settings.xml";
        private const string ROOT_ELEMENT_NAME = "settings";
        private const string ELEMENT_NAME = "setting";
        private const string KEY_ATTRIBUTE_NAME = "key";

        private readonly string fileName;
        private readonly KeyedCollectionFunc<string, IIOSetting> settings;
        private readonly Dictionary<Type, IIOSettingsSerializer> serializers;

        private XmlDocument? xmlDocument;
        public IOSettingsManager(IAppDataManager appDataManager)
        {
            fileName = appDataManager.RoamingPath + FILE_NAME;
            settings = new KeyedCollectionFunc<string, IIOSetting>(s => s.Key);
            serializers = new Dictionary<Type, IIOSettingsSerializer>();
            xmlDocument = Load();
        }

        private void SetAsDefault(XmlDocument xmlDocument)
        {
            xmlDocument.LoadXml($"<{ROOT_ELEMENT_NAME} />");
        }

        private XmlDocument Load()
        {
            var xmlDocument = new XmlDocument();

            if (File.Exists(fileName))
            {
                xmlDocument.Load(fileName);
            }
            else
            {
                SetAsDefault(xmlDocument);
            }

            return xmlDocument;
        }

        public object? GetValue(string key) => settings[key].Value;
        public IIOSetting GetSetting(string key) => settings[key];
        public IEnumerable<IIOSetting> GetSettings() => settings;
        public IEnumerable<IIOSetting> GetSection(string key) => GetSettings().Where(s => s.Key.StartsWith(key));

        public void AddSettings(IEnumerable<IIOSetting> settings)
        {
            if (xmlDocument == null)
                throw new InvalidOperationException("Cannot add settings after application is booted");

            foreach (var setting in settings)
            {
                if (setting.Type == typeof(object))
                    throw new ArgumentException($"A {nameof(IIOSetting)}.{nameof(IIOSetting.Type)} is not allowed to be {nameof(Object)}", nameof(settings));

                this.settings.Add(setting);
                if (xmlDocument.SelectNodes($"/{ROOT_ELEMENT_NAME}/{ELEMENT_NAME}[@{KEY_ATTRIBUTE_NAME}='{setting.Key}']")?.FirstOrDefault() is XmlElement settingNode)
                {
                    setting.Value = FindSerializer(setting.Type).Deserialize(setting.Type, settingNode, this);
                }
                else
                {
                    setting.Value = setting.GetDefaultValue();
                }
            }
        }
        public void AddSettings(params IIOSetting[] settings) => AddSettings((IEnumerable<IIOSetting>)settings);

        public void AddSerializers(IEnumerable<IIOSettingsSerializer> serializers)
        {
            if (serializers.Any(s => s.SupportedTypes.Contains(typeof(object))))
                throw new ArgumentException($"A {nameof(IIOSettingsSerializer)} is not allowed to support type {nameof(Object)}", nameof(serializers));

            foreach(var serializer in serializers)
            {
                foreach(var type in serializer.SupportedTypes)
                {
                    this.serializers.Add(type, serializer);
                }
            }
        }
        public void AddSerializers(params IIOSettingsSerializer[] serializers) => AddSerializers((IEnumerable<IIOSettingsSerializer>)serializers);

        public void Save()
        {
            var xmlDocument = new XmlDocument();
            SetAsDefault(xmlDocument);
            var rootElement = xmlDocument.FirstChild!; //SetAsDefault adds rootElement

            foreach (var setting in settings)
            {
                if (!Object.Equals(setting.Value, setting.GetDefaultValue()))
                {
                    var settingElement = xmlDocument.CreateElement(ELEMENT_NAME);
                    settingElement.SetAttribute(KEY_ATTRIBUTE_NAME, setting.Key);
                    FindSerializer(setting.Type).Serialize(setting.Value, settingElement, this);
                    rootElement.AppendChild(settingElement);
                }
            }

            xmlDocument.Save(fileName);
        }

        public IIOSettingsSerializer FindSerializer(Type type)
        {
            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (serializers.TryGetValue(baseType, out var serializer))
                    return serializer;

                baseType = baseType.BaseType;
            }

            var interfaces = type.GetInterfaces();
            foreach (var intf in interfaces)
            {
                if (serializers.TryGetValue(intf, out var serializer))
                    return serializer;
                if (intf.IsGenericType && serializers.TryGetValue(intf.GetGenericTypeDefinition(), out serializer))
                    return serializer;
            }

            throw new KeyNotFoundException($"Cannot find a {nameof(IIOSettingsSerializer)} that supports type {type.FullName}");
        }

        void IAfterDIContainerBuildListener.AfterDIContainerBuild()
        {
            xmlDocument = null;
        }
    }

    public static class IIOSettingsManagerExtensions {
        public static T GetValue<T>(this IIOSettingsManager manager, string key) where T : notnull => (T)manager.GetValue(key)!;
        public static T GetResolvedValue<T>(this IIOSettingsManager manager, string key) => (T)((IResolveableIOSetting)manager.GetSetting(key)).GetResolvedValue()!;
    }
}
