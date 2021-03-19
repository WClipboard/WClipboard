using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class DictionaryToItemConverter : BaseConverter<object, object>
    {
        public object? Key { get; set; }

        public override object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var key = parameter ?? Key ?? throw new ArgumentNullException($"{nameof(Key)} in {nameof(DictionaryToItemConverter)} is null and also no key provided in the {nameof(parameter)}");

            if (value is IDictionary dictionary)
            {
                return dictionary[key] ?? throw new KeyNotFoundException($"Cannot find key {key} in dictionary");
            }
            else
            {
                return (value.GetType().GetProperty("Item") ?? throw new InvalidOperationException($"{value.GetType().Name} must implement a item property [] to be used in {nameof(DictionaryToItemConverter)}")).GetValue(value, new object[] { key }) ?? throw new KeyNotFoundException($"Cannot find key {key} in {value.GetType().Name}"); ;
            }
        }
    }
}
