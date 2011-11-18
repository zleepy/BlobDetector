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
using Cip.Imaging;
using Cip.Imaging.BlobDetection;
using Cip.Imaging.Tool;

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
                //originalPictureBox.Image = Image.FromFile(openFileDialog1.FileName);
                using (var originalImage = Image.FromFile(openFileDialog1.FileName))
                {
                    totalTime.Stop();
                    Debug.WriteLine("Open image time: " + totalTime.Elapsed.TotalSeconds.ToString());

                    using (var image = FastDirectImage.FromImage(originalImage))
                    {
                        var conversionTime = Stopwatch.StartNew();
                        var gsImage = FastGrayScaleImage.FromImage(image);

                        //new Cip.Imaging.SaveHandlers.PgmHandler().Save(gsImage, @"D:\test.pgm");

                        originalPictureBox.Image = gsImage.NativeBitmap;
                        //using (var gsImage = FastGrayScaleImage.FromImage(image))
                        //{
                        conversionTime.Stop();
                        Console.WriteLine("Convertion to gray scale image: " + conversionTime.Elapsed.TotalSeconds.ToString());

                        UpdateVerticalHistogram(image);
                        UpdateHorizontalHistogram(image);

                        //var b = new SimpleBlobDetector();
                        var b = new BlobDetector(0.6);
                        blobs = b.DetectBlobs(gsImage);

                        var merger = new BlobDistanceMerge(true, 5, new BlobMerger());
                        blobs = merger.Process(blobs);
						
						var blobFilter = new BlobSizeFilter(true, 1, 1, 150, 150, 0.5);
                        blobs = blobFilter.Process(blobs);

                        
                        // R�kna ut baslinje och andra linjer i texten
                        // Identifiera boxar som h�r till samma rad.
                        // Identifiera baslinje och andra linjer f�r raderna.
                        
                        // Skapa typ ett histogram �ver bottnarna p� alla boxar och hitta lokal maxima som �r innom medelh�jden f�r alla boxar.
                        double averageHeight = blobs.Average(box => box.BoundingBox.Height);
                        
                        var bottomPositions = blobs
                            .GroupBy(box => box.BoundingBox.Bottom)
                            .Select(g => new { Y = g.Key, Count = g.Count() })
                            .OrderBy(k => k.Y);

                        int floatingSum = 0;
                        var floatingWindow = new Queue<int>();
                        var maximas = new Dictionary<int, int>();

                        foreach (var item in bottomPositions)
                        {
                            floatingWindow.Enqueue(item.Count);

                            if (floatingWindow.Count > averageHeight)
                                floatingSum -= floatingWindow.Dequeue();

                            floatingSum += item.Count;
                        }

                        
                        //double k = 0;
                        //double c = 0;
                        //LinearLeastSquared.Calculate(blobs.Select(x => new PointF((float)x.BoundingBox.X, (float)x.BoundingBox.Bottom)), ref k, ref c);


                    }
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
                    maskPictureBox.Image = new BlobToImageConverter().Convert(b);
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
}
