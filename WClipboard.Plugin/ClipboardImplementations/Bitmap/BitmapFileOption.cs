using System;
using System.Windows.Media.Imaging;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public class BitmapFileOption
    {
        public string Name { get; }
        public string Extension { get; }
        public Type EncoderType { get; }

        private BitmapFileOption(string name, string extension, Type encoderType)
        {
            Name = name;
            Extension = extension;
            EncoderType = encoderType;
        }

        public static BitmapFileOption Create<TEncoder>(string name = null, string extension = null) where TEncoder : BitmapEncoder, new()
        {
            var encoderType = typeof(TEncoder);
            name ??= encoderType.Name.Substring(0, encoderType.Name.LastIndexOf(nameof(BitmapEncoder)));
            extension ??= name.ToLowerInvariant();

            return new BitmapFileOption(name, extension, encoderType);
        }

        public BitmapEncoder CreateEncoder()
        {
            return (BitmapEncoder)EncoderType.GetConstructor(Array.Empty<Type>()).Invoke(null);
        }
    }
}
