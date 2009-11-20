using NUnit.Framework;
using System;
using Cip.Imaging.BlobDetection;
using System.Drawing;


namespace UnitTests
{
	[TestFixture()]
	public class BlobMergerTests
	{
		[Test()]
		public void Merge_OverlappingBlobs_OneBlobWithBothBlobs()
		{
			// 0 1
			// 1 0
			var b1 = new Blob();
			b1.BoundingBox = new Rectangle(1, 1, 2, 2);
			b1.Area = 2;
			b1.Mask = new byte[] { 96 }; 
			
			// 1 0
			// 0 1
			var b2 = new Blob();
			b2.BoundingBox = new Rectangle(2, 2, 2, 2);
			b2.Area = 2;
			b2.Mask = new byte[] { 144 };
			
			var merger = new BlobMerger();
			var result = merger.Merge(b1, b2);
			
			// 0 1 0
			// 1 1 0
			// 0 0 1
			Assert.AreEqual(4, result.Area);
			Assert.AreEqual(new Rectangle(1, 1, 3, 3), result.BoundingBox);
			Assert.AreEqual(88, result.Mask[0]);
			Assert.AreEqual(128, result.Mask[1]);
		}
	}
}
