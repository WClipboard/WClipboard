using System;
using System.Globalization;
using System.Windows;

namespace WClipboard.Core.WPF.Converters
{
    public class FirstNotNullConverter : BaseMultiConverter
    {
        public override object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            foreach(var value in values)
            {
                if (value != null && value != DependencyProperty.UnsetValue) {
                    return value;
                }
            }

            return null;
        }
    }
}
