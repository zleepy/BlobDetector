using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cip.Imaging.Tool;
using System.Drawing;
using System.Diagnostics;

namespace Cip.Imaging.BlobDetection
{
    public class BlobDistanceMerge
    {
        public bool IsVerbose { get; private set; }
		public int MinDistance { get; private set; }
        public BlobMerger Merger { get; private set; }

        public BlobDistanceMerge(bool isVerbose, int minDistance, BlobMerger merger)
        {
            IsVerbose = isVerbose;
			MinDistance = minDistance;
            Merger = merger;
        }

        public IEnumerable<Blob> Process(IEnumerable<Blob> blobs)
        {
			var result = new List<Blob>(blobs);

            Trace.WriteLineIf(IsVerbose, "BlobDistanceMerge: Antal blobbar: " + result.Count.ToString());

            int blobCount = blobs.Count();
            for (int i = 0; i < blobCount; i++) 
			{
				var r = result[i].BoundingBox;
				r.Inflate(MinDistance, MinDistance);

                for (int k = i + 1; k < blobCount; k++) 
				{
					if(r.IntersectsWith(result[k].BoundingBox))
					{
						result[i] = Merge(result[i], result[k]);
						r = result[i].BoundingBox;
						r.Inflate(MinDistance, MinDistance);
						result.RemoveAt(k);
						k--;
                        blobCount--;
					}
				}
        	}

            Trace.WriteLineIf(IsVerbose, "BlobDistanceMerge: Antal blobbar efter sammanslagning: " + result.Count.ToString());

			return result;
        }
		
		private Blob Merge(Blob a, Blob b)
		{
            Trace.WriteLineIf(IsVerbose, string.Format("BlobDistanceMerge: Sammanslagning av {0} och {1}.", a.BoundingBox, b.BoundingBox));
            return Merger.CreateMergedBlob(a, b);
		}
    }
}
