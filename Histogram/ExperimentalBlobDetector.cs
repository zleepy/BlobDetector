using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Cip.Imaging;
using Cip.Imaging.Tool;

namespace Histogram
{
    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class ExperimentalBlobDetector : IBlobDetectior
    {
        private class Line
        {
            public int LineIndex;
            public int Index;
            public int Count;
        }

        private class LineBlob
        {
            public int Id;
            public List<Line> Lines = null;
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
            Stopwatch resultCreationTime = Stopwatch.StartNew();
            resultCreationTime.Stop();

            int width = img.Width;
            int height = img.Height;

            int blobCount = 0;
            int tmpBlobStart;
            float brightnessValue;

            List<LineBlob> blobs = new List<LineBlob>(4000);
            //List<LineBlob> parentBlobs = new List<LineBlob>(1000);
            List<LineBlob> lastLineBlobs = new List<LineBlob>(width / 2);
            List<LineBlob> tempBlobs = new List<LineBlob>(width / 2);
            LineBlob tempBlob;

            for (int y = 0; y < height; y++)
            {
                blobManagementTime.Start();
                // Håll parentBlob listan kort för att snabba upp sökningar.
                //parentBlobs.RemoveAll(a => !tempBlobs.Exists(b => b.ParentId == a.Id || b.Id == 0));

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

                        int tmpBlobLength = x - tmpBlobStart;

                        lineSearchTime.Stop();
                        mergeTime.Start();

                        tempBlob = null;
                        bool isMerged = false;

                        // Hitta blob att merga med om det finns någon.
                        foreach (var lastLineBlob in lastLineBlobs)
                        {
                            foreach (var line in lastLineBlob.Lines)
                            {
                                //         |------------|          <- Om det här är den föregående raden.
                                // |----|                 |----|   <- Så är det här är de enda vi inte vill koppla ihop med den ovanför.
                                //      |----| |----| |-----|
                                //    |----------------------|
                                if (!(((tmpBlobStart + tmpBlobLength) < line.Index) || ((line.Index + line.Count) < tmpBlobStart)))
                                {
                                    if (!isMerged)
                                    {
                                        lastLineBlob.Lines.Add(new Line() { Index = tmpBlobStart, Count = tmpBlobLength, LineIndex = y });
                                        tempBlob = lastLineBlob;
                                        isMerged = true;
                                    }
                                    else
                                    {
                                        lastLineBlob.Id = tempBlob.Id;
                                    }
                                }
                            }
                        }
                        mergeTime.Stop();
                        lineSearchTime.Start();

                        if (tempBlob == null)
                        {
                            tempBlob = new LineBlob()
                            {
                                Id = blobCount++,
                                Lines = new List<Line>() { new Line() { Index = tmpBlobStart, Count = tmpBlobLength, LineIndex = y } },
                            };
                            blobs.Add(tempBlob);
                        }

                        tempBlobs.Add(tempBlob);
                    }
                }
                lineSearchTime.Stop();
            }

            resultCreationTime.Start();

            var result = new List<Blob>();
            var lines = new List<Line>();
            Rectangle rect;
            int area;
            int top;
            int bottom;
            int left;
            int right;

            // Hämta ut alla blobbar grupperat på id och skapa resultatet utifrån dom.
            foreach (var group in blobs.GroupBy(g => g.Id))
            {
                lines.Clear();
                area = 0;
                top = int.MaxValue;
                bottom = int.MinValue;
                left = int.MaxValue;
                right = int.MinValue;

                foreach (var l in group)
                {
                    lines.AddRange(l.Lines);

                    foreach (var tl in l.Lines)
                    {
                        area += tl.Count;

                        if (tl.Index < left)
                            left = tl.Index;
                        if (tl.Index + tl.Count > right)
                            right = tl.Index + tl.Count;
                        if (tl.LineIndex < top)
                            top = tl.LineIndex;
                        if (tl.LineIndex > bottom)
                            bottom = tl.LineIndex;
                    }
                }

                rect = new Rectangle(left, top, right - left + 1, bottom - top + 1);

                result.Add(new Blob()
                {
                    Area = lines.Sum(l => l.Count),
                    BoundingBox = rect,
                    Mask = CreateMask(rect, lines),
                });
            }

            resultCreationTime.Start();
            totalTime.Stop();

            Debug.WriteLine("-------------------------------------------");
            Debug.WriteLine("Blob count:" + blobs.Count.ToString());
            Debug.WriteLine("Blob result creation time:" + resultCreationTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("Merged blob count:" + result.Count.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob detection time:" + lineSearchTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob merge time:" + mergeTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Blob management time:" + blobManagementTime.Elapsed.TotalSeconds.ToString());
            Debug.WriteLine("BlobDetector:DetectBlobs Total time:" + totalTime.Elapsed.TotalSeconds.ToString());

            return result;
        }

        private byte[] CreateMask(Rectangle rect, IEnumerable<Line> allLines)
        {
            //var allLines = new List<LineBlob>(children);
            //allLines.Add(parent);

            byte[] newMask = new byte[(int)Math.Ceiling(rect.Width * rect.Height / 8.0)];

            foreach (var lineBlob in allLines)
                BitManipulator.SetRange(newMask, (lineBlob.LineIndex - rect.Y) * rect.Width + (lineBlob.Index - rect.X), lineBlob.Count, true);

            return newMask;
        }
    }
}
