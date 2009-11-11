using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cip.Imaging
{
    public interface IBrightnessImage
    {
        int Width { get; }
        int Height { get; }

        float GetBrightness(int x, int y);
    }
}
