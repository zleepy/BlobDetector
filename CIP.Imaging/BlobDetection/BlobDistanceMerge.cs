using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cip.Imaging.BlobDetection
{
    public class BlobDistanceMerge
    {
        public bool IsVerbose { get; private set; }

        public BlobDistanceMerge(bool isVerbose, int minDistance)
        {
            IsVerbose = isVerbose;
        }

        public IEnumerable<Blob> Process(IEnumerable<Blob> blobs)
        {
               
        }
    }
}
