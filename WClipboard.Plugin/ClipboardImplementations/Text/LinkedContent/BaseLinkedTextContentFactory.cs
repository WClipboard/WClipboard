using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public abstract class BaseLinkedTextContentFactory : ILinkedTextContentFactory
    {
        public IEnumerable<Regex> Regexes { get; }

        protected BaseLinkedTextContentFactory(IEnumerable<Regex> regexes)
        {
            Regexes = regexes;
        }

        public abstract Task<ILinkedTextContent?> Create(TextClipboardImplementation textClipboardImplementation, Regex regex, Match match);
    }
}
