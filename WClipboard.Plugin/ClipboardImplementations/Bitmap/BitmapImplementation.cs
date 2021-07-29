using WClipboard.Core.DI;
using WClipboard.Core.WPF.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.IO;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Format;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public class BitmapImplementation : ClipboardImplementation
    {
        private readonly string fileName;

        public IntSize Size { get; }
        private byte[] hash;

        public BitmapImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent, BitmapSource bitmap) : base(format, factory, parent)
        {
            fileName = DiContainer.SP.GetService<ITempManager>().GetNewFileName("bmp");
            Size = new IntSize(bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.Save(fileName, new BmpBitmapEncoder());
        }

        public BitmapImplementation(ClipboardObject clipboardObject, ClipboardImplementationFactory factory, BitmapEquatableFormat source) : base(source.Format, factory, clipboardObject)
        {
            fileName = DiContainer.SP.GetService<ITempManager>().GetNewFileName("bmp");
            Size = source.Size;
            if(source.HasCalculatedHash())
                hash = source.GetHash();
            source.BitmapSource.Save(fileName, new BmpBitmapEncoder());
        }

        internal BitmapImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject, Stream bmpStream) : base(format, factory, clipboardObject)
        {
            fileName = DiContainer.SP.GetService<ITempManager>().GetNewFileName("bmp");
            //TODO set size
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                bmpStream.CopyTo(fileStream);
            }
        }

        public BitmapSource GetImage()
        {
            return LoadBmpImage(fileName, false);
        }

        internal async Task WriteAsBmpToStream(Stream stream)
        {
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            await fileStream.CopyToAsync(stream).ConfigureAwait(false);
        }

        private static Stream LoadStream(string fileName, bool inMemory)
        {
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            if (inMemory)
            {
                var memoryStream = new MemoryStream();
                using (fileStream)
                {
                    fileStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                return memoryStream;
            }
            else
            {
                return fileStream;
            }
        }

        private static BitmapSource LoadBmpImage(string fileName, bool inMemory)
        {
            BitmapDecoder decoder;
            using (var stream = LoadStream(fileName, inMemory))
            {
                decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return decoder.Frames[0];
        }

        public byte[] GetHash()
        {
            if(hash == null)
            {
                var hasher = SHA512.Create();

                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    hash = hasher.ComputeHash(fileStream);
                }
            }
            return hash;
        }

        public override bool IsEqual(EqualtableFormat equaltable)
        {
            if (!(equaltable is BitmapEquatableFormat bitmapEquatable))
                return false;

            if (Size != bitmapEquatable.Size)
                return false;

            if (!Enumerable.SequenceEqual(GetHash(), bitmapEquatable.GetHash()))
                return false;

            return true;
        }
    }
}
