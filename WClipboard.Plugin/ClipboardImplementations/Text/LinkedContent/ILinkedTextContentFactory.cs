using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public interface ILinkedTextContentFactory
    {
        public IEnumerable<Regex> Regexes { get; }

        public Task<ILinkedTextContent?> Create(TextClipboardImplementation textClipboardImplementation, Regex regex, Match match);
    }
}
