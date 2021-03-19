using System;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class SplitConverter : BaseConverter<string, string[]>
    {
        public string? Seperator { get; set; }

        public override string[] Convert(string value, Type targetType, object? parameter, CultureInfo culture)
        {
            var seperator = parameter as string ?? Seperator;

            return value.Split(seperator);
        }
    }
}
