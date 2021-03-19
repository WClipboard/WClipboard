using System;
using WClipboard.Core.Clipboard.Format;

namespace WClipboard.Core.WPF.Clipboard.Implementation
{
    public abstract class EqualtableFormat
    {
        public ClipboardImplementationFactory Factory { get; }
        public Type ImplementationType { get; }
        public ClipboardFormat Format { get; }

        protected EqualtableFormat(ClipboardImplementationFactory factory, Type implementationType, ClipboardFormat format)
        {
            Factory = factory;
            ImplementationType = implementationType;
            Format = format;
        }
    }
}
