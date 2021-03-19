using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.WPF.Utilities
{
    public interface IImageDownloader
    {
        Task<BitmapSource> DownloadImageAsync(string url);
    }

    internal class ImageDownloader : IImageDownloader
    {
        private readonly WeakValueDictionary<string, BitmapSource> cache;

        public ImageDownloader()
        {
            cache = new WeakValueDictionary<string, BitmapSource>(new ConcurrentDictionary<string, WeakReference<BitmapSource>>());
        }

        public async Task<BitmapSource> DownloadImageAsync(string url)
        {
            if (cache.TryGetValue(url, out var bitmapSource))
            {
                return bitmapSource;
            }

            using (var webClient = new WebClient())
            {
                using (var ms = new MemoryStream(await webClient.DownloadDataTaskAsync(url).ConfigureAwait(false)))
                {
                    var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    bitmapSource = decoder.Frames[0];
                    bitmapSource.Freeze();
                    cache[url] = bitmapSource;
                    return bitmapSource;
                }
            }
        }
    }
}
