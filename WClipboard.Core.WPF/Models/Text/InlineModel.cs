using System.Windows;
using System.Windows.Documents;

namespace WClipboard.Core.WPF.Models.Text
{
    public abstract class InlineModel : TextElementModel
    {
        public abstract Inline Create(FrameworkElement coveringElement);
    }
}
