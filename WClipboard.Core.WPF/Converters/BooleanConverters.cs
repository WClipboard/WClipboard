using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WClipboard.Core.WPF.Converters
{
    public class AllConverter : BaseConverter<IEnumerable<bool>, bool>
    {
        public bool Not { get; set; } = false;

        public override bool Convert(IEnumerable<bool> value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Not ^ value.All(x => x);
        }
    }

    public class AnyConverter : BaseConverter<IEnumerable<bool>, bool>
    {
        public bool Not { get; set; } = false;
        public override bool Convert(IEnumerable<bool> value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Not ^ value.Any(x => x);
        }
    }
}
