using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.WPF.Models.Text
{
    public class SpanModel : InlineModel
    {
        public List<InlineModel> Inlines { get; } = new List<InlineModel>();

        public override Inline Create(FrameworkElement coveringElement)
        {
            return Create(new Span(), coveringElement);
        }

        public new T Create<T>(T span, FrameworkElement coveringElement) where T : Span
        {
            ICollectionExtensions.AddRange(span.Inlines, Inlines.Select(i => i.Create(coveringElement)));
            return base.Create(span, coveringElement);
        }
    }
}
