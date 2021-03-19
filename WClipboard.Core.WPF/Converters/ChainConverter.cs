using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.Converters
{
    [ContentProperty("Chain")]
    public class ChainConverter : IValueConverter
    {
        public List<IValueConverter> Chain { get; set; } = new List<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(var converter in Chain)
            {
                value = converter.Convert(value, targetType, parameter, culture);
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(var converter in Enumerable.Reverse(Chain))
            {
                value = converter.ConvertBack(value, targetType, parameter, culture);
            }
            return value;
        }
    }
}
