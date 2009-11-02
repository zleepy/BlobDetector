using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Histogram
{
    public interface IBlobDetectior
    {
        IEnumerable<Blob> DetectBlobs(FastDirectImage img);
    }
}
