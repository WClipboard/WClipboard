using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.Converters
{
    [ContentProperty(nameof(Dictionary))]
    public class KeyToValueConverter : BaseConverter<object, object>
    {
        public IDictionary Dictionary { get; set; } = new Dictionary<object, object>();

        public override object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var dictionary = parameter as IDictionary ?? Dictionary;
                
            return dictionary[value] ?? throw new KeyNotFoundException($"{nameof(value)} is not found as key in {nameof(dictionary)}");
        }

        public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var dictionary = parameter as IDictionary ?? Dictionary;

            var enumerator = dictionary.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Value == value)
                {
                    return enumerator.Key;
                }
            }

            throw new KeyNotFoundException($"{nameof(value)} is not found as value in {nameof(dictionary)}");
        }
    }
}
