using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace WClipboard.Core.WPF.Models.Text
{
    public class HyperlinkModel : SpanModel
    {
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }

        public override Inline Create(FrameworkElement coveringElement)
        {
            return Create(new Hyperlink()
            {
                Command = Command,
                CommandParameter = CommandParameter
            }, coveringElement);
        }
    }
}
