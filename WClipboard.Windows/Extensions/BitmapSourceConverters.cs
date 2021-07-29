using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WClipboard.Windows.Native;

namespace WClipboard.Windows.Extensions
{
    public static class BitmapSourceConverters
    {
        public static BitmapSource? ToBitmapSource(IntPtr hIcon, bool destoryIcon = true) => ToBitmapSource(hIcon, BitmapSizeOptions.FromEmptyOptions(), destoryIcon);

        public static BitmapSource? ToBitmapSource(IntPtr hIcon, BitmapSizeOptions bitmapSizeOptions, bool destoryIcon = true)
        {
            if (bitmapSizeOptions == null)
                throw new ArgumentNullException(nameof(bitmapSizeOptions));

            if (hIcon == IntPtr.Zero)
                return null;

            var img = Imaging.CreateBitmapSourceFromHIcon(
                            hIcon,
                            Int32Rect.Empty,
                            bitmapSizeOptions);

            if (destoryIcon)
                NativeMethods.DestroyIcon(hIcon);

            if (img.CanFreeze)
                img.Freeze();

            return img;
        }

        public static BitmapSource? ToBitmapSource(this System.Drawing.Icon icon) => ToBitmapSource(icon, BitmapSizeOptions.FromEmptyOptions());

        public static BitmapSource? ToBitmapSource(this System.Drawing.Icon icon, BitmapSizeOptions bitmapSizeOptions) => ToBitmapSource(icon.Handle, bitmapSizeOptions, false);

        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            if (bitmapSource.CanFreeze)
                bitmapSource.Freeze();

            return bitmapSource;
        }
    }
}
