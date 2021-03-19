using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WClipboard.Core.Extensions.IO
{
    public static class StreamExtensions
    {
        private static bool TryRead<T>(this Stream stream, Func<byte[], int, T> conv, out T value) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            var buffer = new byte[size];

            if (size == stream.Read(buffer, 0, size))
            {
                value = conv(buffer, 0);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static bool TryReadInt32(this Stream stream, out int value) => TryRead(stream, BitConverter.ToInt32, out value);
        public static bool TryReadInt64(this Stream stream, out long value) => TryRead(stream, BitConverter.ToInt64, out value);
        public static bool TryReadString(this Stream stream, out string? value, Encoding encoding)
        {
            if(stream.TryReadInt32(out var size))
            {
                var buffer = new byte[size];
                if(size == stream.Read(buffer, 0, size))
                {
                    value = encoding.GetString(buffer);
                    return true;
                }
            }
            value = null;
            return false;
        }

        public static void Write(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, Marshal.SizeOf<int>());
        }

        public static void Write(this Stream stream, long value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, Marshal.SizeOf<long>());
        }

        public static void Write(this Stream stream, string value, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            stream.Write(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static bool IsEndOfStream(this Stream stream)
        {
            return stream.Position == stream.Length;
        }

        public static async Task CopyToAsync(this Stream src, Stream dst)
        {
            if(src.CanSeek)
            {
                await CopyToAsync(src, dst, src.Length - src.Position).ConfigureAwait(false);
            }
            else
            {
                byte[] buffer = new byte[0x2000];
                int n;
                do
                {
                    n = await src.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    await dst.WriteAsync(buffer, 0, n).ConfigureAwait(false);
                } while (n > 0);
            }
        }

        public static async Task CopyToAsync(this Stream src, Stream dst, long length)
        {
            byte[] buffer = new byte[Math.Min(0x2000, length)];
            int n;
            do
            {
                n = await src.ReadAsync(buffer, 0, length > buffer.Length ? buffer.Length : (int)length).ConfigureAwait(false);
                await dst.WriteAsync(buffer, 0, n).ConfigureAwait(false);
                length -= n;
            } while (n > 0 && length > 0);
        }

        public static Task CopyToAsync(this Stream src, MemoryStream dst)
        {
            if (src.CanSeek)
            {
                return CopyToAsync(src, dst, src.Length - src.Position);
            }
            else
            {
                return CopyToAsync(src, (Stream)dst);
            }
        }

        public static async Task CopyToAsync(this Stream src, MemoryStream dst, long length)
        {
            if (dst.Position + length > int.MaxValue)
            {
                await CopyToAsync(src, (Stream)dst, length).ConfigureAwait(false);
            }
            else
            {
                var pos = (int)dst.Position;
                length += pos;
                dst.SetLength(length);

                while (pos < length)
                    pos += await src.ReadAsync(dst.GetBuffer(), pos, (int)Math.Min(length - pos, int.MaxValue)).ConfigureAwait(false);
            }
        }

        public static void CopyTo(this Stream src, Stream dest)
        {
            int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 0x2000) : 0x2000;
            byte[] buffer = new byte[size];
            int n;
            do
            {
                n = src.Read(buffer, 0, buffer.Length);
                dest.Write(buffer, 0, n);
            } while (n != 0);
        }

        public static void CopyTo(this MemoryStream src, Stream dest)
        {
            dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
        }

        public static void CopyTo(this Stream src, MemoryStream dest)
        {
            if (src.CanSeek)
            {
                int pos = (int)dest.Position;
                int length = (int)(src.Length - src.Position) + pos;
                dest.SetLength(length);

                while (pos < length)
                    pos += src.Read(dest.GetBuffer(), pos, length - pos);
            }
            else
                src.CopyTo((Stream)dest);
        }

        public static void CopyTo(this MemoryStream src, BinaryWriter dest)
        {
            dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
        }

        public static void CopyTo(this BinaryReader src, MemoryStream dest, long length)
        {
            if (dest.Position + length > dest.Length) {
                dest.SetLength(dest.Position + length);
            }

            src.Read(dest.GetBuffer(), (int)dest.Position, (int)length);
        }
    }
}
