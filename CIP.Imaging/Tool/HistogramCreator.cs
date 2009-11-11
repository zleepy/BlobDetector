using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Cip.Imaging;
using Cip.Imaging.Tool;

namespace Cip.Imaging
{
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

        public int[] CreateVerticalHistogram(byte[] source, int width, int height)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            int[] vhist = new int[height];

            for (int y = 0; y < height; y++)
                vhist[y] = BitManipulator.CountSetBits(source, width * y, width) / width;
            
            Debug.WriteLine("HistogramCreator:CreateVerticalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
            return vhist;
        }

        public int[] CreateHorizontalHistogram(byte[] source, int width, int height)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            int[] hhist = new int[width];

            for (int x = 0; x < width; x++)
            {
                int sum = 0;
                for (int y = 0; y < height; y++)
                    sum += BitManipulator.ToInt32(source, width * y + x);
                hhist[x] = sum / height;
            }
            Debug.WriteLine("HistogramCreator:CreateHorizontalHistogram Total time: " + totalTime.Elapsed.TotalSeconds.ToString());
            return hhist;
        }
    }
}
