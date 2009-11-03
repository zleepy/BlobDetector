using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace Histogram
{
    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class BlobDetector : IBlobDetectior
    {
        public IEnumerable<Blob> DetectBlobs(IBrightnessImage img)
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
            float brightnessValue;
            int lastRowIndex = 0;
            int currentRowIndex = width;

            //int tmpX;

            for (int y = 0; y < height; y++)
            {
                lineSearchTime.Start();

                lastPixelBlobNo = 0;

                int swapTemp = lastRowIndex;
                lastRowIndex = currentRowIndex;
                currentRowIndex = swapTemp;

                for (int x = 0; x < width; x++)
                {
                    brightnessValue = img.GetBrightness(x, y);

                    if (brightnessValue < 0.5)
                    {
                        //lastPixelBlobNo = ++blobCount;
                        //    tempBlob = new List<Point>();
                        //    blobs.Add(lastPixelBlobNo, tempBlob);
                        //for (tmpX = ++x; x < width; x++)
                        //{
                        //    brightnessValue = img.GetPixel(x, y).GetBrightness();

                        //    if (brightnessValue >= 0.5)
                        //        break;

                        //    tempBlob.Add(new Point(x, y));
                        //}


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


    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class BlobDetector2 : IBlobDetectior
    {
        private class LineBlob
        {
            public int Id;
            public int ParentId = 0;
            public List<int> Children = null;

            public int LineIndex;
            public int Index;
            public int Count;
        }

        public IEnumerable<Blob> DetectBlobs(IBrightnessImage img)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            Stopwatch lineSearchTime = Stopwatch.StartNew();
            lineSearchTime.Stop();
            Stopwatch mergeTime = Stopwatch.StartNew();
            mergeTime.Stop();
            Stopwatch blobManagementTime = Stopwatch.StartNew();
            blobManagementTime.Stop();

            int width = img.Width;
            int height = img.Height;

            int blobCount = 0;
            int tmpBlobStart;
            float brightnessValue;

            List<LineBlob> blobs = new List<LineBlob>(4000);
            List<LineBlob> parentBlobs = new List<LineBlob>(1000);
            List<LineBlob> lastLineBlobs = new List<LineBlob>(width / 2);
            List<LineBlob> tempBlobs = new List<LineBlob>(width / 2);
            LineBlob tempBlob;

            for (int y = 0; y < height; y++)
            {
                blobManagementTime.Start();
                parentBlobs.RemoveAll(a => !lastLineBlobs.Exists(b => b.ParentId == a.Id));

                parentBlobs.AddRange(tempBlobs.FindAll(a => a.ParentId == 0));

                lastLineBlobs.Clear();
                lastLineBlobs.AddRange(tempBlobs);

                tempBlobs.Clear();
                blobManagementTime.Stop();

                lineSearchTime.Start();

                for (int x = 0; x < width; x++)
                {
                    brightnessValue = img.GetBrightness(x, y);

                    if (brightnessValue < 0.5)
                    {
                        tmpBlobStart = x;

                        // Hitta slutet på blobben.
                        for (++x; x < width; x++)
                        {
                            brightnessValue = img.GetBrightness(x, y);
                            if (brightnessValue >= 0.5)
                                break;
                        }

                        tempBlob = new LineBlob()
                        {
                            Id = ++blobCount,
                            Index = tmpBlobStart,
                            LineIndex = y,
                            Count = x - tmpBlobStart,
                        };

                        blobs.Add(tempBlob);
                    }
                }
                lineSearchTime.Stop();

                mergeTime.Start();
                // Slå ihob blobbar som har pixlar brevid varandra.
            //    List<Point> blobToMerge;
            //    int currentPixelBlobNo;
            //    for (int x = 0; x < width; x++)
            //    {
            //        currentPixelBlobNo = row[currentRowIndex + x];
            //        lastPixelBlobNo = row[lastRowIndex + x];
            //        if (currentPixelBlobNo > 0 && lastPixelBlobNo > 0 && currentPixelBlobNo != lastPixelBlobNo)
            //        {
            //            blobToMerge = blobs[lastPixelBlobNo];
            //            tempBlob = blobs[currentPixelBlobNo];
            //            tempBlob.AddRange(blobToMerge);
            //            blobs.Remove(lastPixelBlobNo);

            //            // Uppdatera blobindex så att de pekar på den nya blobben.
            //            for (int lx = 0; lx <= x; lx++)
            //            {
            //                if (row[currentRowIndex + lx] == lastPixelBlobNo)
            //                    row[currentRowIndex + lx] = currentPixelBlobNo;
            //            }
            //            for (int lx = 0; lx < width; lx++)
            //            {
            //                if (row[lastRowIndex + lx] == lastPixelBlobNo)
            //                    row[lastRowIndex + lx] = currentPixelBlobNo;
            //            }
            //        }
            //    }
                mergeTime.Stop();
            }

            //var result = blobs.Values.Select(x => new Blob(x));

            totalTime.Stop();

            Debug.WriteLine("BlobDetector:DetectBlobs Blob detection time:" + lineSearchTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob merge time:" + mergeTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob management time:" + blobManagementTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Total time:" + totalTime.Elapsed.TotalSeconds.ToString());

            //return result;
            return null;
        }
    }
}
