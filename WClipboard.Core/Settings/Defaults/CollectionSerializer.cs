using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WClipboard.Core.Settings.Defaults
{
    public class CollectionSerializer : IIOSettingsSerializer
    {
        private const string ITEMNAME = "item";

        public IEnumerable<Type> SupportedTypes => new[] { typeof(ICollection<>) };

        public object? Deserialize(Type type, XmlElement settingElement, IIOSettingsManager settingsManager)
        {
            var collection = (type.GetConstructor(Array.Empty<Type>()) ?? throw new InvalidOperationException($"{nameof(CollectionSerializer)} cannot create an instance of type {type.FullName} using a default constructor")).Invoke(null);
            var itemType = GetItemType(type);
            var itemSerializer = settingsManager.FindSerializer(itemType);
            var addMethod = type.GetMethod("Add", new[] { itemType }) ?? throw new InvalidOperationException($"{nameof(CollectionSerializer)} cannot find an Add method for item type {itemType.FullName} on type {type.FullName}"); 
            foreach (var childElement in settingElement.ChildNodes.OfType<XmlElement>().Where(e => e.Name == ITEMNAME))
            {
                addMethod.Invoke(collection, new[] { itemSerializer.Deserialize(itemType, childElement, settingsManager) });
            }

            return collection;
        }

        public void Serialize(object? value, XmlElement settingElement, IIOSettingsManager settingsManager)
        {
            if (value is null)
            {
                return;
            } 
            else if (value is IEnumerable enumerable)
            {
                var itemType = GetItemType(value.GetType());
                var itemSerializer = settingsManager.FindSerializer(itemType);

                foreach (var itemValue in enumerable)
                {
                    var childElement = settingElement.OwnerDocument.CreateElement(ITEMNAME);
                    itemSerializer.Serialize(itemValue, childElement, settingsManager);
                    settingElement.AppendChild(childElement);
                }
            }
            else
            {
                throw new InvalidOperationException($"{nameof(CollectionSerializer)} cannot serialize an object of type {value.GetType().FullName}");
            }
        }

        private static Type GetItemType(Type type)
        {
            return type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault() ?? 
                throw new InvalidOperationException($"{nameof(CollectionSerializer)} cannot find item type of type {type.FullName}");
        }
    }
}
