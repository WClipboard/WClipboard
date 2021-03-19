using System.Collections.Generic;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.Clipboard.Format { 
    public interface IClipboardFormatCategoriesManager : ICollectionManager<string, ClipboardFormatCategory>
    {
    }

    public class ClipboardFormatCategoriesManager : CollectionManager<string, ClipboardFormatCategory>, IClipboardFormatCategoriesManager
    {
        public ClipboardFormatCategoriesManager(IEnumerable<ClipboardFormatCategory> items) : base(cfc => cfc.Name, items)
        {
        }
    }
}
