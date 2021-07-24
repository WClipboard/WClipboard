using System.IO;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public class BitmapEquatableFormat : EqualtableFormat
    {
        public BitmapSource BitmapSource { get; }
        public IntSize Size { get; }

        private byte[] hash = null;

        public BitmapEquatableFormat(ClipboardFormat format, BitmapSource bitmapSource) : base(format)
        {
            BitmapSource = bitmapSource;
            Size = new IntSize(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
        }

        public byte[] GetHash()
        {
            if (hash == null)
            {
                var hasher = SHA512.Create();

                using (Stream memoryStream = new MemoryStream())
                {
                    BitmapSource.Save(memoryStream, new BmpBitmapEncoder(), false);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    hash = hasher.ComputeHash(memoryStream);
                }
            }

            return hash;
        }

        public bool HasCalculatedHash()
        {
            return hash != null;
        }
    }
}
