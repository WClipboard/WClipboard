using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.Extensions
{
    public class ItemsHost : MarkupExtension
    {
        public int AncestorLevel { get; set; } = 1;

        public ItemsHost() { }
        public ItemsHost(int ancestorLevel) => AncestorLevel = ancestorLevel;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ItemsControl), AncestorLevel).ProvideValue(serviceProvider);
        }
    }
}
