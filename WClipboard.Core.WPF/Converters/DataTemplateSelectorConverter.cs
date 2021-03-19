using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using WClipboard.Core.WPF.DataTemplateSelectors;

namespace WClipboard.Core.WPF.Converters
{
    public class DataTemplateSelectorConverter : BaseConverter<object, DataTemplate>
    {
        public DataTemplateSelector? DataTemplateSelector { get; set; }

        public override DataTemplate Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var templateSelector = parameter as DataTemplateSelector ?? DataTemplateSelector ?? new TypeTemplateSelector();

            return templateSelector.SelectTemplate(value, parameter as DependencyObject);
        }
    }
}
