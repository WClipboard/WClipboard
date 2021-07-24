using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Plugin.ClipboardImplementations.Bitmap;
using WClipboard.Plugin.ClipboardImplementations.Path;
using WClipboard.Plugin.ClipboardImplementations.Text;

namespace WClipboard.Plugin.Defaults
{
    public class DefaultFormatsExtractor : IFormatsExtractor
    {
        public IEnumerable<EqualtableFormat> Extract(ClipboardTrigger trigger, IDataObject dataObject)
        {
            if (dataObject.TryGetData(DefaultClipboardFormats.Unicode.Format, out var unicode) && unicode is string unicodeText)
            {
                yield return new TextEquatableFormat(DefaultClipboardFormats.Unicode, unicodeText);
            }
            if (dataObject.TryGetData(DefaultClipboardFormats.FileDrop.Format, out var fileDrop) && fileDrop is string[] paths)
            {
                yield return new PathsEquatableFormat(DefaultClipboardFormats.FileDrop, paths);
            }
            if (dataObject.TryGetData(DefaultClipboardFormats.Bitmap.Format, out var bitmap) && bitmap is BitmapSource bitmapSource)
            {
                yield return new BitmapEquatableFormat(DefaultClipboardFormats.Bitmap, bitmapSource);
            }
        }
    }
}
