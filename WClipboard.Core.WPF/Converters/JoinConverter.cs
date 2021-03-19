using System;
using System.Collections.Generic;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class JoinConverter : BaseConverter<IEnumerable<string>, string>
    {
        public string? Seperator { get; set; }

        public override string Convert(IEnumerable<string> value, Type targetType, object? parameter, CultureInfo culture)
        {
            var seperator = parameter as string ?? Seperator;

            return string.Join(seperator, value);
        }
    }
}
