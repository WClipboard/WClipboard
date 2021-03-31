using System.Collections.Generic;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public interface IBitmapFileOptionsManager : IReadOnlyKeyedCollection<string, BitmapFileOption> { }

    public class BitmapFileOptionsManager : KeyedCollectionFunc<string, BitmapFileOption>, IBitmapFileOptionsManager
    {
        public BitmapFileOptionsManager(IEnumerable<BitmapFileOption> items) : base((bfo) => bfo.Name, items)
        {
        }
    }
}
