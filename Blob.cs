using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Histogram
{
    public class Blob
    {
        /// <summary>
        /// En rektangel som ramar in blobben.
        /// </summary>
        public Rectangle BoundingBox;

        /// <summary>
        /// Antalet svarta pixlar som ingår i blobben.
        /// </summary>
        public uint Area;

        /// <summary>
        /// En 1bpp mask av vad som hör till den här blobben.
        /// Även om två blobbar ligger inom varandras regione så syns dom inte i varandras mask.
        /// </summary>
        public byte[] Mask;

        public Blob()
        {
        }

        public Blob(List<Point> points)
        {
            Area = (uint)points.Count;
            BoundingBox = CreateBoundingBox(points);
            Mask = CreateMask(BoundingBox, points);
        }

        private byte[] CreateMask(Rectangle rect, List<Point> points)
        {
            byte[] newMask = new byte[(int)Math.Ceiling(rect.Width * rect.Height / 8.0)];

            foreach (var point in points)
                BitManipulator.Set(newMask, (point.Y - rect.Y) * rect.Width + (point.X - rect.X), true);

            return newMask;
        }

        private Rectangle CreateBoundingBox(List<Point> points)
        {
            int left = points[0].X;
            int right = points[0].X;
            int top = points[0].Y;
            int bottom = points[0].Y;

            for (int i = 1; i < points.Count; i++)
            {
                Point p = points[i];
                if (p.X < left)
                    left = p.X;
                if (p.X > right)
                    right = p.X;
                if (p.Y < top)
                    top = p.Y;
                if (p.Y > bottom)
                    bottom = p.Y;
            }

            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }
    }
}
