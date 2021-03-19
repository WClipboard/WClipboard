using System.Text.RegularExpressions;

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public abstract class LinkedTextContent
    {
        public Capture Capture { get; }

        protected LinkedTextContent(Capture capture)
        {
            Capture = capture;
        }
    }
}
