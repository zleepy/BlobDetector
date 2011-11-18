using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Cip.Imaging.BlobDetection
{
    /// <summary>
    /// Filtrerar bort blobbar som inte håller sig innom godkänd storlek.
    /// </summary>
    public class BlobSizeFilter
    {
        public bool IsVerbose { get; private set; }
        public int MinWidth { get; private set; }
        public int MinHeight { get; private set; }
        public int MaxWidth { get; private set; }
        public int MaxHeight { get; private set; }
        public double MaxAverageDeviation { get; private set; }

        public BlobSizeFilter(bool verboseOutput, int minWidth, int minHeight, int maxWidth, int maxHeight, double maxAverageDeviation)
        {
            IsVerbose = verboseOutput;
            MinWidth = minWidth;
            MinHeight = minHeight;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            MaxAverageDeviation = maxAverageDeviation;
        }

        public IEnumerable<Blob> Process(IEnumerable<Blob> blobs)
        {
#if DEBUG
            var timer = Stopwatch.StartNew();
#endif

            if(IsVerbose)
                Trace.WriteLine("BlobSizeFilter.Process: Antal ingående blobbar: " + blobs.Count());

            var result = blobs.Where(b => b.BoundingBox.Width >= MinWidth && b.BoundingBox.Width <= MaxWidth && b.BoundingBox.Height >= MinHeight && b.BoundingBox.Height <= MaxHeight).ToArray();

            if (IsVerbose)
                Trace.WriteLine(string.Format("BlobSizeFilter.Process: Antal blobbar inom måtten {0}x{1} - {2}x{3} är {4}: ", MinWidth, MinHeight, MaxWidth, MaxHeight, result.Count()));

            double averageWidth = result.Average(b => b.BoundingBox.Width);
            double averageHeight = result.Average(b => b.BoundingBox.Height);

            //int upperAverageWidth = (int)(averageWidth * (1.0 + MaxAverageDeviation));
            //int upperAverageHeight = (int)(averageHeight * (1.0 + MaxAverageDeviation));
            //int lowerAverageWidth = (int)(averageWidth * MaxAverageDeviation);   //TODO: Det här blir helt fel.
            //int lowerAverageHeight = (int)(averageHeight * MaxAverageDeviation); //TODO: Det här blir helt fel.

            if (IsVerbose)
                Trace.WriteLine(string.Format("BlobSizeFilter.Process: Medelstorlek {0}x{1}: ", averageWidth, averageHeight));

            //result = result.Where(b => b.BoundingBox.Width >= lowerAverageWidth && b.BoundingBox.Width <= upperAverageWidth && b.BoundingBox.Height >= lowerAverageHeight && b.BoundingBox.Height <= upperAverageHeight);

            //if (IsVerbose)
            //    Trace.WriteLine(string.Format("BlobSizeFilter.Process: Antal blobbar inom måtten {0}x{1} - {2}x{3} är {4}: ", lowerAverageWidth, lowerAverageHeight, upperAverageWidth, upperAverageHeight, result.Count()));

#if DEBUG
            Debug.WriteLine("BlobSizeFilter.Process: Total tid: " + timer.Elapsed.TotalSeconds.ToString() + "s");
#endif

            return result;
        }
    }
}
