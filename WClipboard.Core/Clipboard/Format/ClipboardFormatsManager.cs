using System;
using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.Clipboard.Format
{
    public interface IClipboardFormatsManager : ICollectionManager<string, ClipboardFormat>
    {
        IEnumerable<string> FilterUnknownFormats(IEnumerable<string> formats);
        IEnumerable<ClipboardFormat> GetFormats(IEnumerable<string> formats);
    }

    public class ClipboardFormatsManager : CollectionManager<string, ClipboardFormat>, IClipboardFormatsManager
    {
        public ClipboardFormatsManager(IEnumerable<ClipboardFormat> items, IClipboardFormatCategoriesManager categoriesManager) : base(cf => cf.Name, items)
        {
            foreach (var item in this)
            {
                if (!categoriesManager.Contains(item.Category))
                {
                    throw new ArgumentException($"{nameof(ClipboardFormat)} with name {item.Name} has an {nameof(ClipboardFormat.Category)} what isn\'t registered in the {nameof(ClipboardFormatsManager)}. Please register the category or use the same reference as in the manager", nameof(items));
                }
            }
        }

        public IEnumerable<string> FilterUnknownFormats(IEnumerable<string> formats)
        {
            return formats.Intersect(((IEnumerable<ClipboardFormat>)this).Select(f => f.Format));
        }

        public IEnumerable<ClipboardFormat> GetFormats(IEnumerable<string> formats) {
            return ((IEnumerable<ClipboardFormat>)this).Where(f => formats.Contains(f.Format));
        }
    }
}
