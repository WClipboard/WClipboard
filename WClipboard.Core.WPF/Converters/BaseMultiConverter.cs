using System;
using System.Globalization;
using System.Windows.Data;

namespace WClipboard.Core.WPF.Converters
{
    public abstract class BaseMultiConverter : IMultiValueConverter
    {
        public abstract object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture);

        public virtual object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Cannot convert back in a {this.GetType().Name}");
        }
    }
}
