using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Histogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        IEnumerable<Blob> blobs = null;

        private void openButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stopwatch totalTime = Stopwatch.StartNew();
                originalPictureBox.Image = Image.FromFile(openFileDialog1.FileName);
                totalTime.Stop();
                Debug.WriteLine("Open image time: " + totalTime.Elapsed.TotalSeconds.ToString());

                using (var image = FastDirectImage.FromImage(originalPictureBox.Image))
                {
                    UpdateVerticalHistogram(image);
                    UpdateHorizontalHistogram(image);

                    var b = new BlobDetector();
                    blobs = new List<Blob>(b.DetectBlobs(image));
                }
            }
        }

        private void UpdateVerticalHistogram(FastDirectImage image)
        {
            var h = new HistogramCreator();
            double[] vhist = h.CreateVerticalHistogram(image);

            Bitmap b = new Bitmap(100, image.Height, PixelFormat.Format16bppRgb555);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
                for (int i = 0; i < vhist.Length; i++)
                    g.DrawLine(Pens.White, 0, i, (float)vhist[i] * (float)100, i);
                g.Flush();
            }

            verticalHistPictureBox.Image = b;
        }

        private void UpdateHorizontalHistogram(FastDirectImage image)
        {
            var h = new HistogramCreator();
            double[] hhist = h.CreateHorizontalHistogram(image);

            Bitmap b = new Bitmap(image.Width, 100, PixelFormat.Format16bppRgb555);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
                for (int i = 0; i < hhist.Length; i++)
                    g.DrawLine(Pens.White, i, 0, i, (float)hhist[i] * (float)100);
                g.Flush();
            }

            horizontalHistPictureBox.Image = b;
        }

        private void originalPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (blobs != null)
            {
                float xMultiplier = (float)originalPictureBox.Width / (float)originalPictureBox.Image.Width;
                float yMultiplier = (float)originalPictureBox.Height / (float)originalPictureBox.Image.Height;

                var g = e.Graphics;
                foreach (var b in blobs)
                    g.DrawRectangle(Pens.Blue, b.BoundingBox.X * xMultiplier, b.BoundingBox.Y * yMultiplier, b.BoundingBox.Width * xMultiplier, b.BoundingBox.Height * yMultiplier);
            }
        }

        private void originalPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (blobs != null)
            {
                float xMultiplier = (float)originalPictureBox.Image.Width / (float)originalPictureBox.Width;
                float yMultiplier = (float)originalPictureBox.Image.Height / (float)originalPictureBox.Height;

                Blob b = GetSmallestIntersectedBlob(new Point((int)(e.X * xMultiplier), (int)(e.Y * yMultiplier)));

                if (b != null)
                    maskPictureBox.Image = new ConvertBlobToImage().Convert(b);
            }
        }

        private Blob GetSmallestIntersectedBlob(Point location)
        {
            Blob foundBlob = null;
            foreach (var b in blobs)
            {
                if (b.BoundingBox.Contains(location) && (foundBlob == null || b.Area < foundBlob.Area))
                    foundBlob = b;
            }
            return foundBlob;
        }
    }

    public class ConvertBlobToImage
    {
        public Image Convert(Blob blob)
        {
            Bitmap img = new Bitmap(blob.BoundingBox.Width, blob.BoundingBox.Height, PixelFormat.Format24bppRgb);

            int width = blob.BoundingBox.Width;
            int height = blob.BoundingBox.Height;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    img.SetPixel(x, y, (BitManipulator.ToInt32(blob.Mask, y * width + x) == 0) ? Color.White : Color.Black);

            return img;
        }
    }

    public class HistogramCreator
    {
        public double[] CreateVerticalHistogram(FastDirectImage img)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            int width = img.Width;
            int height = img.Height;
            double[] vhist = new double[height];

            for (int y = 0; y < height; y++)
            {
                double sum = 0;
                for (int x = 0; x < width; x++)
                    sum += img.GetPixel(x, y).GetBrightness();
                vhist[y] = sum / width;
            }
            Debug.WriteLine("HistogramCreator:CreateVerticalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
            return vhist;
        }

        public double[] CreateHorizontalHistogram(FastDirectImage img)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            int width = img.Width;
            int height = img.Height;
            double[] hhist = new double[width];

            for (int x = 0; x < width; x++)
            {
                double sum = 0;
                for (int y = 0; y < height; y++)
                    sum += img.GetPixel(x, y).GetBrightness();
                hhist[x] = sum / height;
            }
            Debug.WriteLine("HistogramCreator:CreateHorizontalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
            return hhist;
        }

        //public int[] CreateVerticalHistogram(Blob img)
        //{
        //    Stopwatch totalTime = Stopwatch.StartNew();
        //    int width = img.BoundingBox.Width;
        //    int height = img.BoundingBox.Height;
        //    int[] vhist = new int[height];

        //    for (int y = 0; y < height; y++)
        //    {
        //        int sum = 0;
        //        for (int x = 0; x < width; x++)
        //            sum += BitManipulator.ToInt32(img.Mask, width * y + y);
        //        vhist[y] = sum / width;
        //    }
        //    Debug.WriteLine("HistogramCreator:CreateVerticalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
        //    return vhist;
        //}

        //public int[] CreateHorizontalHistogram(Blob img)
        //{
        //    Stopwatch totalTime = Stopwatch.StartNew();
        //    int width = img.BoundingBox.Width;
        //    int height = img.BoundingBox.Height;
        //    int[] hhist = new int[width];

        //    for (int x = 0; x < width; x++)
        //    {
        //        int sum = 0;
        //        for (int y = 0; y < height; y++)
        //            sum += BitManipulator.ToInt32(img.Mask, width * y + y);
        //        hhist[x] = sum / height;
        //    }
        //    Debug.WriteLine("HistogramCreator:CreateHorizontalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
        //    return hhist;
        //}
    }

    public class FastDirectImage : IDisposable
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
            byte b = bits[index++];
            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
