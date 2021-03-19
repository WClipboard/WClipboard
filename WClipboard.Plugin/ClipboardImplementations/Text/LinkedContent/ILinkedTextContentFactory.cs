using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public interface ILinkedTextContentFactory
    {
        public IEnumerable<Regex> Regexes { get; }

        public Task<object> Create(TextClipboardImplementation textClipboardImplementation, Regex regex, Match match);
    }
}
