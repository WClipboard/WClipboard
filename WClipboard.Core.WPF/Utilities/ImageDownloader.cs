using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.WPF.Utilities
{
    public interface IImageDownloader
    {
        Task<BitmapSource> DownloadImageAsync(string url);
    }

    internal class ImageDownloader : IImageDownloader
    {
        private readonly WeakValueDictionary<string, BitmapSource> cache;
        private readonly Lazy<HttpClient> httpClient = new Lazy<HttpClient>(() => new HttpClient());

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

            using (var stream = await httpClient.Value.GetStreamAsync(url, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    bitmapSource = decoder.Frames[0];
                }
            }

            bitmapSource.Freeze();
            cache[url] = bitmapSource;
            return bitmapSource;
        }
    }
}
