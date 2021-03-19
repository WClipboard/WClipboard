using System;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.Extensions
{
    [MarkupExtensionReturnType(typeof(bool?))]
    public class BoolExtension : StaticExtension
    {
        public bool? Value { get; set; }

        public BoolExtension() { }

        public BoolExtension(bool? value)
        {
            Value = value;
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
