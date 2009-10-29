using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace Histogram
{
    public interface IBlobDetectionAlgoritm
    {
        IEnumerable<Blob> DetectBlobs(FastDirectImage img);
    }

    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class BlobDetector : IBlobDetectionAlgoritm
    {
        public IEnumerable<Blob> DetectBlobs(FastDirectImage img)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            Stopwatch lineSearchTime = Stopwatch.StartNew();
            Stopwatch mergeTime = Stopwatch.StartNew();

            int width = img.Width;
            int height = img.Height;
            int lastPixelBlobNo;
            int blobCount = 0;
            int[] row = new int[2 * width];
            var blobs = new Dictionary<int, List<Point>>();
            List<Point> tempBlob;

            int lastRowIndex = 0;
            int currentRowIndex = width;

            for (int y = 0; y < height; y++)
            {
                lineSearchTime.Start();

                lastPixelBlobNo = 0;

                int swapTemp = lastRowIndex;
                lastRowIndex = currentRowIndex;
                currentRowIndex = swapTemp;

                for (int x = 0; x < width; x++)
                {
                    float value = img.GetPixel(x, y).GetBrightness();
                    //Debug.WriteLine(string.Format("X:{0} Y:{1} = {2}", x, y, value));

                    if (value < 0.5)
                    {
                        if (lastPixelBlobNo > 0)
                        {
                            tempBlob = blobs[lastPixelBlobNo];
                        }
                        else
                        {
                            lastPixelBlobNo = ++blobCount;
                            tempBlob = new List<Point>();
                            blobs.Add(lastPixelBlobNo, tempBlob);
                        }
                        tempBlob.Add(new Point(x, y));
                    }
                    else
                    {
                        lastPixelBlobNo = 0;
                    }
                    row[currentRowIndex + x] = lastPixelBlobNo;
                }
                lineSearchTime.Stop();

                mergeTime.Start();
                // Slå ihob blobbar som har pixlar brevid varandra.
                List<Point> blobToMerge;
                int currentPixelBlobNo;
                for (int x = 0; x < width; x++)
                {
                    currentPixelBlobNo = row[currentRowIndex + x];
                    lastPixelBlobNo = row[lastRowIndex + x];
                    if (currentPixelBlobNo > 0 && lastPixelBlobNo > 0 && currentPixelBlobNo != lastPixelBlobNo)
                    {
                        blobToMerge = blobs[lastPixelBlobNo];
                        tempBlob = blobs[currentPixelBlobNo];
                        tempBlob.AddRange(blobToMerge);
                        blobs.Remove(lastPixelBlobNo);

                        // Uppdatera blobindex så att de pekar på den nya blobben.
                        for (int lx = 0; lx <= x; lx++)
                        {
                            if (row[currentRowIndex + lx] == lastPixelBlobNo)
                                row[currentRowIndex + lx] = currentPixelBlobNo;
                        }
                        for (int lx = 0; lx < width; lx++)
                        {
                            if (row[lastRowIndex + lx] == lastPixelBlobNo)
                                row[lastRowIndex + lx] = currentPixelBlobNo;
                        }
                    }
                }
                mergeTime.Stop();
            }

            var result = blobs.Values.Select(x => new Blob(x));

            totalTime.Stop();

            Debug.WriteLine("BlobDetector:DetectBlobs Blob detection time:" + lineSearchTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob merge time:" + mergeTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Total time:" + totalTime.Elapsed.TotalSeconds.ToString());

            return result;
        }
    }
}
