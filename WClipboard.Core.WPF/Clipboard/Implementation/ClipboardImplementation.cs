using System;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.WPF.Clipboard.Implementation
{
    public abstract class ClipboardImplementation : BindableBase
    {
        public ClipboardFormat Format { get; }
        public ClipboardImplementationFactory Factory { get; }

        public ClipboardImplementation? Parent { get; }

        private readonly ClipboardObject? _clipboardObject;
        public ClipboardObject ClipboardObject
        {
            get => _clipboardObject ?? Parent?.ClipboardObject ?? throw new ApplicationException("Should never happen");
        }

        protected ClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent)
        {
            Format = format;
            Factory = factory;
            Parent = parent;
        }

        protected ClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject)
        {
            Format = format;
            Factory = factory;
            _clipboardObject = clipboardObject;
        }

        public abstract bool IsEqual(EqualtableFormat equaltable);
    }
}
