using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cip.Imaging;

namespace Histogram
{
    public interface IBlobDetectior
    {
        IEnumerable<Blob> DetectBlobs(IBrightnessImage img);
    }
}
