using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cip.Imaging.Tool;
using System.Diagnostics;

namespace Cip.Imaging.BlobDetection
{
    public class BlobMerger
    {
        public Blob Merge(Blob a, Blob b)
        {
            var result = new Blob();
            result.BoundingBox = Rectangle.Union(a.BoundingBox, b.BoundingBox);
            result.Mask = CreateMergedMask(a, b);
            result.Area = BitManipulator.CountSetBits(result.Mask);
            return result;
        }

        public byte[] CreateMergedMask(Blob a, Blob b)
        {
            var resultRect = Rectangle.Union(a.BoundingBox, b.BoundingBox);
            var result = new byte[(int)Math.Ceiling((resultRect.Width * resultRect.Height) / 8.0)];

            AddToMask(result, resultRect, a.Mask, a.BoundingBox);
            AddToMask(result, resultRect, b.Mask, b.BoundingBox);

            return result;
        }

        public void AddToMask(byte[] destination, Rectangle destLocation, byte[] source, Rectangle sourceLocation)
        {
            int xdiff = sourceLocation.X - destLocation.X;
            int ydiff = sourceLocation.Y - destLocation.Y;
            //int totalDiff = ydiff + xdiff;

            for (int y = 0; y < sourceLocation.Height; y++)
                BitManipulator.And(destination, xdiff + (destLocation.Width * (y + ydiff)), sourceLocation.Width, source, sourceLocation.Width * y);
        }
    }
}
