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
                        blobToMerge = blobs[currentPixelBlobNo];
                        tempBlob = blobs[lastPixelBlobNo];
                        tempBlob.AddRange(blobToMerge);
                        blobs.Remove(currentPixelBlobNo);

                        // Uppdatera blobindex så att de pekar på den nya blobben.
                        for (int lx = 0; lx < width; lx++)
                        {
                            if (row[currentRowIndex + lx] == currentPixelBlobNo)
                                row[currentRowIndex + lx] = lastPixelBlobNo;
                        }
                        for (int lx = x; lx < width; lx++)
                        {
                            if (row[lastRowIndex + lx] == currentPixelBlobNo)
                                row[lastRowIndex + lx] = lastPixelBlobNo;
                        }
                    }
                }
                mergeTime.Stop();
            }

            var result = blobs.Values.Select(x => new Blob(x));

            totalTime.Stop();

            Debug.WriteLine("-------------------------------------------");
			Debug.WriteLine("Blob count:" + blobCount.ToString());
			Debug.WriteLine("Merged blob count:" + blobs.Count.ToString());
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
        //private class Line
        //{
        //    public int LineIndex;
        //    public int Index;
        //    public int Count;
        //}

        private class LineBlob
        {
            public int Id;
            public int ParentId = 0;
            public List<int> Children = null;
            //public List<Line> Children = null;

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
            Stopwatch resultCreationTime = Stopwatch.StartNew();
            resultCreationTime.Stop();

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
                // Håll parentBlob listan kort för att snabba upp sökningar.
                parentBlobs.RemoveAll(a => !tempBlobs.Exists(b => b.ParentId == a.Id || b.Id == 0));

                //parentBlobs.AddRange(tempBlobs.FindAll(a => a.ParentId == 0));

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

                mergeTime.Start();
				
				foreach (var currentLineBlob in tempBlobs) 
				{
					foreach (var lastLineBlob in lastLineBlobs) 
					{
                        //         |------------|          <- Om det här är den föregående raden.
                        // |----|                 |----|   <- Så är det här är de enda vi inte vill koppla ihop med den ovanför.
                        //      |----| |----| |-----|
                        //    |----------------------|
						if(!(((currentLineBlob.Index + currentLineBlob.Count) < lastLineBlob.Index) || ((lastLineBlob.Index + lastLineBlob.Count) < currentLineBlob.Index)))
						{
							//if(lastLineBlob.Children == null)
							//	lastLineBlob.Children = new List<int>();
							
							if(lastLineBlob.ParentId == 0)
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

            resultCreationTime.Start();

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



    /// <summary>
    /// http://geekblog.nl/entry/24
    /// </summary>
    public class BlobDetector3 : IBlobDetectior
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
