using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace Histogram
{
    public class FastGrayScaleImage : IDisposable, IBrightnessImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Image NativeBitmap { get { return image.NativeBitmap; } }

        public float GetBrightness(int x, int y)
        {
            int index = y * image.Stride + x * image.PixelFormatSize;
            return image.Bits[index] / 256.0f;
        }

        private FastDirectImage image;

        public static FastGrayScaleImage FromImage(FastDirectImage img)
        {
            int width = img.Width;
            int height = img.Height;

            var result = new FastGrayScaleImage();
            result.image = FastDirectImage.Create(width, height, PixelFormat.Format8bppIndexed);

            var palette = result.NativeBitmap.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            result.NativeBitmap.Palette = palette;

            int oIndex = -img.Padding;
            int dIndex = -result.image.Padding;
            int dIndexNexLine;
            int r;
            int g;
            int b;

            while (dIndex < (height * result.image.Stride - result.image.Padding))
            //for (int y = 0; y < height; y++)
            {
                oIndex += img.Padding;
                dIndex += result.image.Padding;
                dIndexNexLine = dIndex + result.image.Stride - result.image.Padding;

                while(dIndex < dIndexNexLine)
                {
                    r = img.Bits[oIndex];
                    g = img.Bits[oIndex + 1];
                    b = img.Bits[oIndex + 2];

                    result.image.Bits[dIndex] = (byte)((b * 0.11) + (g * 0.59) + (r * 0.3));
                    
                    oIndex += img.PixelFormatSize;
                    dIndex += result.image.PixelFormatSize;
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (image != null)
                image.Dispose();
        }
    }
}
