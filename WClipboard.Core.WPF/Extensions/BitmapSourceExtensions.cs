using System.IO;
using System.Windows.Media.Imaging;

#nullable enable

namespace WClipboard.Core.WPF.Extensions
{
    public static class BitmapSourceExtensions
    {
        public static void Save(this BitmapSource source, Stream stream, BitmapEncoder encoder, bool closeStream = false)
        {
            if (stream is null)
                throw new System.ArgumentNullException(nameof(stream));

            try
            {
                if (source is null)
                    throw new System.ArgumentNullException(nameof(source));
                if (encoder is null)
                    throw new System.ArgumentNullException(nameof(encoder));

                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
            }
            finally
            {
                if (closeStream)
                    stream.Dispose();
            }
        }

        public static void Save(this BitmapSource source, string fileName, BitmapEncoder encoder, bool overwrite = true)
        {
            if (source is null)
                throw new System.ArgumentNullException(nameof(source));
            if (fileName is null)
                throw new System.ArgumentNullException(nameof(fileName));
            if (encoder is null)
                throw new System.ArgumentNullException(nameof(encoder));

            using (var stream = File.Open(fileName, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
            }
        }
    }
}
