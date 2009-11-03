using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;

namespace Histogram
{
    public class FastGrayScaleImage : IDisposable, IBrightnessImage
    {
        public int Width { get { return NativeBitmap.Width; } }
        public int Height { get { return NativeBitmap.Height; } }
        public Image NativeBitmap { get { return image.NativeBitmap; } }

        public float GetBrightness(int x, int y)
        {
            int index = y * image.Stride + x * image.PixelFormatSize;
			
			//Debug.WriteLine(string.Format("GetBrightness: {0}x{1}[{2}] = {3}", Width, Height, index, image.Bits[index] / 256.0f));
			
            return image.Bits[index] / 256.0f;
        }

        private FastDirectImage image;

        public static FastGrayScaleImage FromImage(FastDirectImage img)
        {
            int width = img.Width;
            int height = img.Height;

            var result = new FastGrayScaleImage();
            result.image = FastDirectImage.Create(width, height, PixelFormat.Format8bppIndexed);
            CreateGrayScalePalette(result.NativeBitmap);

            int oIndex = -img.Padding;
            int dIndex = -result.image.Padding;
            int dIndexLineLenght = result.image.Stride - result.image.Padding;
            int dIndexEoi = (height * result.image.Stride) - result.image.Padding;
            int dIndexNextLine;
            int r;
            int g;
            int b;
            
            int imgPixelFormatSize = img.PixelFormatSize;
            int resultPixelFormatSize = result.image.PixelFormatSize;
            
            byte[] imgBytes = img.Bits;
            byte[] resultBytes = result.image.Bits;

            while (dIndex < dIndexEoi)
            {
                oIndex += img.Padding;
                dIndex += result.image.Padding;
                dIndexNextLine = dIndex + dIndexLineLenght;

                while(dIndex < dIndexNextLine)
                {
                    r = imgBytes[oIndex];
                    g = imgBytes[oIndex + 1];
                    b = imgBytes[oIndex + 2];

                    resultBytes[dIndex] = (byte)((b * 0.11) + (g * 0.59) + (r * 0.3));

                    oIndex += imgPixelFormatSize;
                    dIndex += resultPixelFormatSize;
                }
            }

            return result;
        }

        private static void CreateGrayScalePalette(Image img)
        {
            var palette = img.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            img.Palette = palette;
        }

        public void Dispose()
        {
            if (image != null)
                image.Dispose();
        }
    }
}
