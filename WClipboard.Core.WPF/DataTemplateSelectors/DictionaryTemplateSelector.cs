using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.DataTemplateSelectors
{
    [ContentProperty(nameof(Dictionary))]
    public class DictionaryTemplateSelector : DataTemplateSelector
    {
        public Dictionary<object, DataTemplate> Dictionary { get; set; } = new Dictionary<object, DataTemplate>();

        public override DataTemplate? SelectTemplate(object? item, DependencyObject? container)
        {
            if (item is null)
                return null;

            if (Dictionary.TryGetValue(item, out var dataTemplate))
                return dataTemplate;

            if (container is FrameworkElement feContainer && feContainer.TryFindResource<DataTemplate>(item, out var resource))
                return resource;

            if (Application.Current.TryFindResource<DataTemplate>(item, out resource))
                return resource;

            return null;
        }
    }
}
