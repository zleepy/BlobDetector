using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using Cip.Imaging;
using Cip.Imaging.Tool;

namespace Histogram
{
    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class BlobDetector : IBlobDetectior
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
            Trace.WriteLine("-------------------------------------------");

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

            List<LineBlob> allBlobs = new List<LineBlob>(4000);
            List<LineBlob> activeParentBlobs = new List<LineBlob>(1000);
            List<LineBlob> lastLineBlobs = new List<LineBlob>(width / 2);
            List<LineBlob> currentLineBlobs = new List<LineBlob>(width / 2);

            for (int y = 0; y < height; y++)
            {
                blobManagementTime.Start();
                // Håll parentBlob listan kort för att snabba upp sökningar.
                activeParentBlobs.RemoveAll(a => !currentLineBlobs.Exists(b => b.ParentId == a.Id || b.Id == 0));

                lastLineBlobs.Clear();
                lastLineBlobs.AddRange(currentLineBlobs);
                currentLineBlobs.Clear();

                blobManagementTime.Stop();

                blobCount = ScanLine(blobCount, img, lineSearchTime, width, allBlobs, currentLineBlobs, y);

                MergeBlobsWithPreviousLine(mergeTime, activeParentBlobs, lastLineBlobs, currentLineBlobs);
            }

            var result = CreateResultBlobs(allBlobs);

            totalTime.Stop();

            Trace.WriteLine("BlobDetector:DetectBlobs Blob detection time:" + lineSearchTime.Elapsed.TotalSeconds.ToString());
            Trace.WriteLine("BlobDetector:DetectBlobs Blob merge time:" + mergeTime.Elapsed.TotalSeconds.ToString());
            Trace.WriteLine("BlobDetector:DetectBlobs Blob management time:" + blobManagementTime.Elapsed.TotalSeconds.ToString());
            Trace.WriteLine("BlobDetector:DetectBlobs Total time:" + totalTime.Elapsed.TotalSeconds.ToString());

            return result;
        }

        private static int ScanLine(int blobCount, IBrightnessImage img, Stopwatch lineSearchTime, int width, List<LineBlob> blobs, List<LineBlob> tempBlobs, int y)
        {
            int tmpBlobStart;
            float brightnessValue;
            LineBlob tempBlob;

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
                        Id = blobCount++,
                        Index = tmpBlobStart,
                        LineIndex = y,
                        Count = x - tmpBlobStart,
                    };

                    tempBlobs.Add(tempBlob);
                    blobs.Add(tempBlob);
                }
            }
            lineSearchTime.Stop();
            return blobCount;
        }

        private static void MergeBlobsWithPreviousLine(Stopwatch mergeTime, List<LineBlob> parentBlobs, List<LineBlob> lastLineBlobs, List<LineBlob> currentLineBlobs)
        {
            mergeTime.Start();

            foreach (var currentLineBlob in currentLineBlobs)
            {
                foreach (var lastLineBlob in lastLineBlobs)
                {
                    //         |------------|          <- Om det här är den föregående raden.
                    // |----|                 |----|   <- Så är det här är de enda vi inte vill koppla ihop med den ovanför.
                    //      |----| |----| |-----|
                    //    |----------------------|
                    if (!(((currentLineBlob.Index + currentLineBlob.Count) < lastLineBlob.Index) || ((lastLineBlob.Index + lastLineBlob.Count) < currentLineBlob.Index)))
                    {
                        if (lastLineBlob.ParentId == 0)
                        {
                            if (lastLineBlob.Children == null)
                            {
                                lastLineBlob.Children = new List<int>();
                                parentBlobs.Add(lastLineBlob);
                            }

                            currentLineBlob.ParentId = lastLineBlob.Id;
                            lastLineBlob.Children.Add(currentLineBlob.Id);
                        }
                        else
                        {
                            var parentBlob = parentBlobs.Find(p => p.Id == lastLineBlob.ParentId);
                            parentBlob.Children.Add(currentLineBlob.Id);
                            currentLineBlob.ParentId = parentBlob.Id;
                        }
                    }
                }
            }

            mergeTime.Stop();
        }

        private List<Blob> CreateResultBlobs(List<LineBlob> blobs)
        {
            Stopwatch resultCreationTime = Stopwatch.StartNew();

            var allParentBlobs = blobs.FindAll(a => a.ParentId == 0);
            var result = new List<Blob>();

            foreach (var pb in allParentBlobs)
            {
                int area = pb.Count;
                int top = pb.LineIndex;
                int bottom = pb.LineIndex;
                int left = pb.Index;
                int right = pb.Index + pb.Count;

                var children = (pb.Children == null) ? new List<LineBlob>() : pb.Children.ConvertAll(c => blobs[c]);

                foreach (var c in children)
                {
                    area += c.Count;

                    if (c.Index < left)
                        left = c.Index;
                    if (c.Index + c.Count > right)
                        right = c.Index + c.Count;
                    if (c.LineIndex < top)
                        top = c.LineIndex;
                    if (c.LineIndex > bottom)
                        bottom = c.LineIndex;
                }

                var rectangle = new Rectangle(left, top, right - left, bottom - top + 1);

                result.Add(new Blob()
                {
                    Area = area,
                    BoundingBox = rectangle,
                    Mask = CreateMask(rectangle, pb, children),
                });
            }
            resultCreationTime.Stop();

            Trace.WriteLine("Blob count:" + blobs.Count.ToString());
            Trace.WriteLine("Blob result creation time:" + resultCreationTime.Elapsed.TotalSeconds.ToString());
            Trace.WriteLine("Merged blob count:" + result.Count.ToString());

            return result;
        }

        private byte[] CreateMask(Rectangle rect, LineBlob parent, IEnumerable<LineBlob> children)
        {
            var allLines = new List<LineBlob>(children);
            allLines.Add(parent);

            byte[] newMask = new byte[(int)Math.Ceiling(rect.Width * rect.Height / 8.0)];

            foreach (var lineBlob in allLines)
                BitManipulator.SetRange(newMask, (lineBlob.LineIndex - rect.Y) * rect.Width + (lineBlob.Index - rect.X), lineBlob.Count, true);

            return newMask;
        }
    }
}
