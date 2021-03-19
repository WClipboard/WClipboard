using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class ContainsConverter : BaseConverter<string, bool>
    {
        public string? Value { get; set; }

        public override bool Convert(string value, Type targetType, object? parameter, CultureInfo culture)
        {
            var search = parameter as string ?? Value ?? throw new InvalidOperationException($"{nameof(Value)} or a string {nameof(parameter)} must be set in order to use the {nameof(ContainsConverter)}");

            return value.Contains(search);
        }
    }
}
