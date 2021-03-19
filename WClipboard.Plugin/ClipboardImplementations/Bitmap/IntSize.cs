using System;

namespace WClipboard.Plugin.ClipboardImplementations.Bitmap
{
    public struct IntSize : IEquatable<IntSize>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public IntSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public static bool operator ==(IntSize left, IntSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntSize left, IntSize right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntSize size)
                Equals(size);

            return base.Equals(obj);
        }

        public bool Equals(IntSize size)
        {
            return size.Height == Height && size.Width == Width;
        }

        public override string ToString()
        {
            return $"[W:{Width}, H:{Height}]";
        }
    }
}
