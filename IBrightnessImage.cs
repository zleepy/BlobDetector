using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Histogram
{
    public interface IBrightnessImage
    {
        int Width { get; }
        int Height { get; }

        float GetBrightness(int x, int y);
    }
}
