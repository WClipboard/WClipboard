using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Format;

namespace WClipboard.Core.WPF.Clipboard.Implementation
{
    public abstract class ClipboardImplementationFactory
    {
        public abstract IEnumerable<EqualtableFormat> CreateEquatables(DataObject dataObject);
        public abstract Task<ClipboardImplementation> CreateFromEquatable(ClipboardObject clipboardObject, EqualtableFormat equaltableFormat);
        public abstract Task<ClipboardImplementation> CreateLinkedFromDataObject(ClipboardImplementation parent, DataObject dataObject);
        public abstract Task WriteToDataObject(ClipboardImplementation implementation, DataObject dataObject);
        public abstract Task Serialize(ClipboardImplementation implementation, Stream stream);
        public abstract Task<IEnumerable<ClipboardImplementation>> Deserialize(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format);
    }
}
