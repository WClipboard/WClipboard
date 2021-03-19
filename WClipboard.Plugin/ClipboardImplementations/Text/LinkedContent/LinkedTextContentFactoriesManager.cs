using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent;

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public class LinkedTextContentFactoriesManager : BaseLinkedContentFactoriesManager<TextClipboardImplementation>, IEnumerable<ILinkedTextContentFactory>
    {
        private readonly List<ILinkedTextContentFactory> factories;

        public LinkedTextContentFactoriesManager(IEnumerable<ILinkedTextContentFactory> factories)
        {
            this.factories = new List<ILinkedTextContentFactory>(factories);
            foreach (var f in this.factories.Where(f => !f.Regexes.All(r => r.Options.HasFlag(RegexOptions.Compiled)))) {
                throw new InvalidOperationException($"{nameof(ILinkedTextContentFactory)} of type {f.GetType().Name} must have all {nameof(ILinkedTextContentFactory.Regexes)} with flag {nameof(RegexOptions)}.{nameof(RegexOptions.Compiled)} set to ensure performance");
            }
        }

        protected override Task ProvideAsync(TextClipboardImplementation textImplementation, IClipboardObjectManager clipboardObjectManager)
        {
            return Task.WhenAll(factories.Select(f => Task.Run(() => CheckFactoryAsync(textImplementation, clipboardObjectManager, f))));
        }

        private async Task CheckFactoryAsync(TextClipboardImplementation textImplementation, IClipboardObjectManager clipboardObjectManager, ILinkedTextContentFactory factory)
        {
            foreach (var regex in factory.Regexes)
            {
                var matches = regex.Matches(textImplementation.Source);
                if (matches.Count > 0)
                {
                    //TODO make matches distinct
                    foreach (Match match in matches)
                    {
                        clipboardObjectManager.AddLinkedContent(textImplementation, await factory.Create(textImplementation, regex, match).ConfigureAwait(false));
                    }
                }
            }
        }

        public IEnumerator<ILinkedTextContentFactory> GetEnumerator() => ((IEnumerable<ILinkedTextContentFactory>)factories).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ILinkedTextContentFactory>)factories).GetEnumerator();
    }
}
