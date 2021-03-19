using System.Windows;
using System.Windows.Documents;

namespace WClipboard.Core.WPF.Models.Text
{
    public class RunModel : InlineModel
    {
        public string? Text { get; set; }

        public override Inline Create(FrameworkElement coveringElement)
        {
            return Create(new Run(Text), coveringElement);
        }
    }
}
