using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Plugin.ClipboardImplementations.Bitmap;
using WClipboard.Plugin.ClipboardImplementations.Path;
using WClipboard.Plugin.ClipboardImplementations.Text;

namespace WClipboard.Plugin.Defaults
{
    internal class DefaultClipboardImplementationFactory : ClipboardImplementationFactory
    {
        public override Task<ClipboardImplementation> CreateFromEquatable(ClipboardObject clipboardObject, EqualtableFormat equaltableFormat)
        {
            return Task.FromResult<ClipboardImplementation>(
                equaltableFormat switch {
                TextEquatableFormat textEquatableFormat => new TextClipboardImplementation(clipboardObject, this, textEquatableFormat),
                PathsEquatableFormat pathsEquatableFormat => new PathsImplementation(clipboardObject, this, pathsEquatableFormat),
                BitmapEquatableFormat bitmapEquatableFormat => new BitmapImplementation(clipboardObject, this, bitmapEquatableFormat),
                _ => null,
            });
        }

        public override Task<ClipboardImplementation> CreateLinkedFromDataObject(ClipboardImplementation parent, DataObject dataObject)
        {
            ClipboardImplementation implementation = null;

            if (dataObject.TryGetData(DefaultClipboardFormats.Unicode.Format, out var unicode) && unicode is string unicodeText)
            {
                implementation = new TextClipboardImplementation(DefaultClipboardFormats.Unicode, this, parent, unicodeText);
            }
            else if (dataObject.TryGetData(DefaultClipboardFormats.FileDrop.Format, out var fileDrop) && fileDrop is string[] paths)
            {
                implementation = new PathsImplementation(DefaultClipboardFormats.FileDrop, this, parent, paths);
            }
            else if (dataObject.TryGetData(DefaultClipboardFormats.Bitmap.Format, out var bitmap) && bitmap is BitmapSource bitmapSource)
            {
                implementation = new BitmapImplementation(DefaultClipboardFormats.Bitmap, this, parent, bitmapSource);
            }

            return Task.FromResult(implementation);
        }

        public override Task WriteToDataObject(ClipboardImplementation implementation, DataObject dataObject)
        {
            switch(implementation)
            {
                case TextClipboardImplementation textImplementation:
                    dataObject.SetData(textImplementation.Format.Format, textImplementation.Source);
                    break;
                case PathsImplementation pathsImplementation:
                    dataObject.SetFileDropList(pathsImplementation.Paths);
                    break;
                case BitmapImplementation bitmapImplementation:
                    dataObject.SetImage(bitmapImplementation.GetImage());
                    break;
            }

            return Task.CompletedTask;
        }

        public override async Task Serialize(ClipboardImplementation implementation, Stream stream)
        {
            switch (implementation)
            {
                case TextClipboardImplementation textImplementation:
                    using (var sw = new StreamWriter(stream, System.Text.Encoding.Unicode, leaveOpen: true))
                    {
                        await sw.WriteAsync(textImplementation.Source);
                    }
                    break;
                case PathsImplementation pathsImplementation:
                    using (var sw = new StreamWriter(stream, System.Text.Encoding.Unicode, leaveOpen: true))
                    {
                        await sw.WriteAsync(string.Join("\n", pathsImplementation.Paths));
                    }
                    break;
                case BitmapImplementation bitmapImplementation:
                    await bitmapImplementation.WriteAsBmpToStream(stream);
                    break;
            }
        }

        public override async Task<IEnumerable<ClipboardImplementation>> Deserialize(ClipboardObject clipboardObject, Stream stream, ClipboardFormat format)
        {
            var returner = new List<ClipboardImplementation>();

            if(format == DefaultClipboardFormats.Unicode)
            {
                using (var sr = new StreamReader(stream, System.Text.Encoding.Unicode, leaveOpen: true))
                {
                    string unicodeText = await sr.ReadToEndAsync();
                    returner.Add(new TextClipboardImplementation(DefaultClipboardFormats.Unicode, this, clipboardObject, unicodeText));
                }
            }

            if(format == DefaultClipboardFormats.FileDrop)
            {
                using (var sr = new StreamReader(stream, System.Text.Encoding.Unicode, leaveOpen: true))
                {
                    string paths = await sr.ReadToEndAsync();
                    returner.Add(new PathsImplementation(DefaultClipboardFormats.FileDrop, this, clipboardObject, paths.Split("\n")));
                }
            }

            if(format == DefaultClipboardFormats.Bitmap)
            {
                returner.Add(new BitmapImplementation(DefaultClipboardFormats.Bitmap, this, clipboardObject, stream));
            }

            return returner;
        }
    }
}
