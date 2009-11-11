using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Cip.Imaging.Tool;

namespace Histogram
{
    public class BlobToImageConverter
    {
        public Image Convert(Blob blob)
        {
            Bitmap img = new Bitmap(blob.BoundingBox.Width, blob.BoundingBox.Height, PixelFormat.Format24bppRgb);

            int width = blob.BoundingBox.Width;
            int height = blob.BoundingBox.Height;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    img.SetPixel(x, y, (BitManipulator.ToInt32(blob.Mask, y * width + x) == 0) ? Color.White : Color.Black);

            return img;
        }
    }
}
