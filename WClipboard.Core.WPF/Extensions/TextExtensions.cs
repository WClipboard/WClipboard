using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WClipboard.Core.Extensions;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Models.Text;

namespace WClipboard.Core.WPF.Extensions
{
    public static class TextExtensions
    {
        public static readonly DependencyProperty InlinesProperty = DependencyProperty.RegisterAttached("Inlines", typeof(IEnumerable<InlineModel>), typeof(TextExtensions), new FrameworkPropertyMetadata(OnInlinesChanged));

        public static void SetInlines(DependencyObject target, IEnumerable<InlineModel> value)
        {
            target.SetValue(InlinesProperty, value);
        }

        public static IEnumerable<InlineModel> GetInlines(DependencyObject target)
        {
            return (IEnumerable<InlineModel>)target.GetValue(InlinesProperty);
        }

        private static void OnInlinesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBlock textBlock)
            {
                textBlock.Inlines.Clear();
                if (e.NewValue is IEnumerable<InlineModel> inlines)
                {
                    ICollectionExtensions.AddRange(textBlock.Inlines, inlines.Select(i => i.Create(textBlock)));
                }
            } 
            else if (obj is Paragraph paragraph)
            {
                paragraph.Inlines.Clear();
                if (e.NewValue is IEnumerable<InlineModel> inlines && 
                    RecursiveEnumerable.While(paragraph.Parent, obj => obj is FrameworkContentElement fce, obj => ((FrameworkContentElement)obj).Parent).LastOrDefault() is FrameworkElement fe)
                {
                    ICollectionExtensions.AddRange(paragraph.Inlines, inlines.Select(i => i.Create(fe)));
                }
            }
        }
    }
}
