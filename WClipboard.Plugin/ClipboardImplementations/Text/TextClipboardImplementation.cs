﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class TextClipboardImplementation : ClipboardImplementation
    {
        public string Source { get; }

        public ConcurrentObservableList<ILinkedTextContent>? LinkedContent { get; }

        public TextClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent, string source) : base(format, factory, parent)
        {
            Source = source;
            LinkedContent = null;
        }

        public TextClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject, string source) : base(format, factory, clipboardObject)
        {
            Source = source;
            LinkedContent = new ConcurrentObservableList<ILinkedTextContent>();
            Task.Run(() => DiContainer.SP!.GetRequiredService<LinkedTextContentFactoriesManager>().ProvideAsync(this));
        }

        public TextClipboardImplementation(ClipboardObject clipboardObject, ClipboardImplementationFactory factory, TextEquatableFormat source) : base(source.Format, factory, clipboardObject)
        {
            Source = source.Text;
            LinkedContent = new ConcurrentObservableList<ILinkedTextContent>();
            Task.Run(() => DiContainer.SP!.GetRequiredService<LinkedTextContentFactoriesManager>().ProvideAsync(this));
        }

        public override bool IsEqual(EqualtableFormat equaltable)
        {
            if (!(equaltable is TextEquatableFormat textEquatable))
                return false;

            if (Source.Length != textEquatable.Text.Length)
                return false;

            return Source == textEquatable.Text;
        }
    }
}
