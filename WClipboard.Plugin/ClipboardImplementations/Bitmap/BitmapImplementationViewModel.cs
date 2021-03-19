using System.Windows.Media;
using System.Windows.Media.Imaging;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public class BitmapImplementationViewModel : ClipboardImplementationViewModel<BitmapImplementation>
    {
        public BitmapSource Thumbnail { get; }
        public int DefaultThumbnailPixelHeight { get; } = 300;

        public BitmapImplementationViewModel(BitmapImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {
            var bitmap = implementation.GetImage();
            if (bitmap.PixelHeight > DefaultThumbnailPixelHeight)
            {
                var scale = (double)DefaultThumbnailPixelHeight / bitmap.PixelHeight;
                Thumbnail = new TransformedBitmap(bitmap, new ScaleTransform(scale, scale));
            }
            else
            {
                Thumbnail = bitmap;
            }

            if (Thumbnail.CanFreeze)
                Thumbnail.Freeze();
        }
    }
}
