using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cip.Imaging.Tool;
using System.Drawing;

namespace Cip.Imaging.BlobDetection
{
    public class BlobDistanceMerge
    {
        public bool IsVerbose { get; private set; }
		public int MinDistance { get; private set; }

        public BlobDistanceMerge(bool isVerbose, int minDistance)
        {
            IsVerbose = isVerbose;
			MinDistance = minDistance;
        }

        public IEnumerable<Blob> Process(IEnumerable<Blob> blobs)
        {
			var result = new List<Blob>(blobs);
        	
			for (int i = 0; i < blobs.Count(); i++) 
			{
				var r = result[i].BoundingBox;
				r.Inflate(MinDistance, MinDistance);
				
				for (int k = i + 1; k < blobs.Count(); k++) 
				{
					if(r.IntersectsWith(result[k].BoundingBox))
					{
						result[i] = Merge(result[i], result[k]);
						r = result[i].BoundingBox;
						r.Inflate(MinDistance, MinDistance);
						result.RemoveAt(k);
						k--;
					}
				}
        	}
			
			return result;
        }
		
		private Blob Merge(Blob a, Blob b)
		{
			Blob result = new Blob();
			result.Mask = BitManipulator.And(a.Mask, b.Mask);
			result.Area = BitManipulator.CountSetBits(result.Mask);
			result.BoundingBox = new Rectangle(
				Math.Min(a.BoundingBox.X, b.BoundingBox.X),
			    Math.Min(a.BoundingBox.Y, b.BoundingBox.Y),
			    Math.Min(a.BoundingBox.Right, b.BoundingBox.Right) - Math.Min(a.BoundingBox.X, b.BoundingBox.X),
			    Math.Min(a.BoundingBox.Bottom, b.BoundingBox.Bottom) - Math.Min(a.BoundingBox.Y, b.BoundingBox.Y));
			
			return result;
		}
    }
}
