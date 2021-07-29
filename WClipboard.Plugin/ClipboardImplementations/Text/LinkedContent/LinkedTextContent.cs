using System.Text.RegularExpressions;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public interface ILinkedTextContent
    {
        /// <summary>
        /// The capture of the text matched
        /// </summary>
        Capture Capture { get; }
        /// <summary>
        /// The model of the found linked content
        /// </summary>
        object Model { get; }
        /// <summary>
        /// A textual description of the kind of the found linked content
        /// </summary>
        string Kind { get; }
    }

    public class LinkedTextContent<TValue> : ILinkedTextContent where TValue : notnull
    {
        public Capture Capture { get; }

        public TValue Model { get; }

        object ILinkedTextContent.Model => Model;

        public string Kind { get; }

        public LinkedTextContent(Capture capture, TValue value, string kind)
        {
            Capture = capture;
            Model = value;
            Kind = kind;
        }
    }
}
