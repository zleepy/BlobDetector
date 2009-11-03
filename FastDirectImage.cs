using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Histogram
{
    public class FastDirectImage : IDisposable, IBrightnessImage
    {
        GCHandle handle;
        private byte[] bits;

        public byte[] Bits { get { return bits; } }
        public Bitmap NativeBitmap { get; private set; }
        public PixelFormat PixelFormat { get; private set; }
        public int PixelFormatSize { get; private set; }
        public int BitPerPixel { get; private set; }
        public int Stride { get; private set; }
        public int Padding { get; private set; }
        public int Width { get { return NativeBitmap.Width; } }
        public int Height { get { return NativeBitmap.Height; } }

        public static FastDirectImage Create(int width, int height, PixelFormat pixelFormat)
        {
            var newImg = new FastDirectImage();
            newImg.PixelFormat = pixelFormat;
            newImg.BitPerPixel = Image.GetPixelFormatSize(pixelFormat);
            newImg.PixelFormatSize = newImg.BitPerPixel / 8;

            // Gör inte såhär för att räkna ut Stride: newImg.Stride = width * newImg.PixelFormatSize; 
            // Detta då stride för bitmappar måste alignbara med 32 bitar (4 byte). (Antagligen för att förenkla kopiering av rader)
            // http://www.tech-archive.net/Archive/DotNet/microsoft.public.dotnet.framework.drawing/2006-09/msg00057.html

            //In your loop, you copy the pixels one scanline at a time and take into
            //consideration the amount of padding that occurs due to memory alignment.
            newImg.Stride = ((width * newImg.BitPerPixel + 31) & ~31) >> 3;
            newImg.Padding = newImg.Stride - (((width * newImg.BitPerPixel) + 7) / 8);

            newImg.bits = new byte[newImg.Stride * height];
            newImg.handle = GCHandle.Alloc(newImg.bits, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(newImg.bits, 0);
            newImg.NativeBitmap = new Bitmap(width, height, newImg.Stride, pixelFormat, pointer);

            // Kan kanske vara något för 
            //var b = newImg.NativeBitmap.LockBits(new Rectangle(0, 0, newImg.NativeBitmap.Width, newImg.NativeBitmap.Height), ImageLockMode.ReadWrite, pixelFormat);

            return newImg;
        }

        public static FastDirectImage FromImage(Image source)
        {
            if (source.PixelFormat == PixelFormat.Indexed)
                throw new ArgumentException("FastDirectImage::FromImage does not support indexed colors.");

            var newImg = Create(source.Width, source.Height, source.PixelFormat);

            using (Graphics g = Graphics.FromImage(newImg.NativeBitmap))
            {
                g.DrawImageUnscaledAndClipped(source, new Rectangle(0, 0, source.Width, source.Height));
                g.Dispose();
            }

            return newImg;
        }

        public void Dispose()
        {
            if (NativeBitmap != null)
                NativeBitmap.Dispose();

            if (handle != null && handle.IsAllocated)
                handle.Free();
        }

        public Color GetPixel(int x, int y)
        {
            //    if(NativeBitmap.PixelFormat == PixelFormat.Alpha)
            int index = y * Stride + x * PixelFormatSize;
            byte r = bits[index++];
            byte g = bits[index++];
            byte b = bits[index];
            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        public float GetBrightness(int x, int y)
        {
            return GetPixel(x, y).GetBrightness();
        }
    }
}
