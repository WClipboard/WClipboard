using System.Windows;
using System.Windows.Controls;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Managers;
using System;

namespace WClipboard.Core.WPF.DataTemplateSelectors
{
    public class TypeTemplateSelector : DataTemplateSelector
    {
        private readonly Lazy<ITypeDataTemplateManager> _manager = DiContainer.SP!.GetLazy< ITypeDataTemplateManager>();

        public override DataTemplate? SelectTemplate(object? item, DependencyObject? container)
        {
            if (item is null)
                return null;

            var itemType = item as Type ?? item.GetType();

            do
            {
                if (itemType is null)
                    return null;
                else if (_manager.Value.TryGetValue(itemType, out var value))
                    return value;
                else if (itemType.IsGenericType && _manager.Value.TryGetValue(itemType.GetGenericTypeDefinition(), out value))
                    return value;

                itemType = itemType.BaseType;
            } while (itemType != typeof(object));

            return null;
        }
    }
}
