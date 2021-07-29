using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Implementation;

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent
{
    public class LinkedTextContentFactoriesManager : IEnumerable<ILinkedTextContentFactory>
    {
        private readonly List<ILinkedTextContentFactory> factories;
        private readonly IClipboardObjectManager clipboardObjectManager;

        public LinkedTextContentFactoriesManager(IEnumerable<ILinkedTextContentFactory> factories, IClipboardObjectManager clipboardObjectManager)
        {
            this.clipboardObjectManager = clipboardObjectManager;
            this.factories = new List<ILinkedTextContentFactory>(factories);
            foreach (var f in this.factories.Where(f => !f.Regexes.All(r => r.Options.HasFlag(RegexOptions.Compiled)))) {
                throw new InvalidOperationException($"{nameof(ILinkedTextContentFactory)} of type {f.GetType().Name} must have all {nameof(ILinkedTextContentFactory.Regexes)} with flag {nameof(RegexOptions)}.{nameof(RegexOptions.Compiled)} set to ensure performance");
            }
        }

        public Task ProvideAsync(TextClipboardImplementation textImplementation)
        {
            return Task.WhenAll(factories.Select(f => Task.Run(() => CheckFactoryAsync(textImplementation, f))));
        }

        private async Task CheckFactoryAsync(TextClipboardImplementation textImplementation, ILinkedTextContentFactory factory)
        {
            foreach (var regex in factory.Regexes)
            {
                var matches = regex.Matches(textImplementation.Source);
                if (matches.Count > 0)
                {
                    //TODO make matches distinct
                    foreach (Match match in matches)
                    {
                        var content = await factory.Create(textImplementation, regex, match).ConfigureAwait(false);
                        if (content.Model is DataObject dataObject)
                        {
                            await foreach(var implementation in clipboardObjectManager.CreateLinkedImplementationsAsync(dataObject, textImplementation)) {
                                textImplementation.LinkedContent.Add(new LinkedTextContent<ClipboardImplementation>(content.Capture, implementation, content.Kind));
                            }
                        } else {
                            textImplementation.LinkedContent.Add(content);
                        }
                    }
                }
            }
        }

        public IEnumerator<ILinkedTextContentFactory> GetEnumerator() => ((IEnumerable<ILinkedTextContentFactory>)factories).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ILinkedTextContentFactory>)factories).GetEnumerator();
    }
}
