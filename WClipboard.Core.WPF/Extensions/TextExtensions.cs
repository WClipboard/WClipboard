using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Models.Text;

namespace WClipboard.Core.WPF.Extensions
{
    public static class TextExtensions
    {
        public static readonly DependencyProperty InlinesProperty = DependencyProperty.RegisterAttached("Inlines", typeof(IEnumerable<InlineModel>), typeof(TextExtensions), new FrameworkPropertyMetadata(OnInlinesChanged));

        public static void SetInlines(FrameworkElement target, IEnumerable<InlineModel> value)
        {
            target.SetValue(InlinesProperty, value);
        }

        public static IEnumerable<InlineModel> GetInlines(FrameworkElement target)
        {
            return (IEnumerable<InlineModel>)target.GetValue(InlinesProperty);
        }

        private static void OnInlinesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(obj is TextBlock textBlock)
            {
                textBlock.Inlines.Clear();
                if(e.NewValue is IEnumerable<InlineModel> inlines)
                {
                    ICollectionExtensions.AddRange(textBlock.Inlines, inlines.Select(i => i.Create((FrameworkElement)obj)));
                }
            }
        }
    }
}
