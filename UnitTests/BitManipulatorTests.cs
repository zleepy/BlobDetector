using NUnit.Framework;
using System;
using Cip.Imaging.Tool;


namespace UnitTests
{
	[TestFixture()]
	public class BitManipulatorTests
	{
		[Test()]
		public void ToBoolean_FirstAndLastBitIsSet_FirstIsTrue()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(true, BitManipulator.ToBoolean(source, 0));
		}
		
		[Test()]
		public void ToBoolean_FirstAndLastBitIsSet_LastIsTrue()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(true, BitManipulator.ToBoolean(source, 15));
		}
		
		[Test()]
		public void ToBoolean_FirstAndLastBitIsSet_SecondIsFalse()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(false, BitManipulator.ToBoolean(source, 1));
		}
		
		[Test()]
		public void ToInt32_FirstAndLastBitIsSet_FirstIsOne()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(1, BitManipulator.ToInt32(source, 0));
		}
		
		[Test()]
		public void ToInt32_FirstAndLastBitIsSet_SecondIsZero()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(0, BitManipulator.ToInt32(source, 1));
		}
		
		[Test()]
		public void ToInt16_FirstAndLastBitIsSet_FirstIsOne()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(1, BitManipulator.ToInt16(source, 0));
		}
		
		[Test()]
		public void ToInt16_FirstAndLastBitIsSet_SecondIsZero()
		{
			byte[] source = {128, 1};
			Assert.AreEqual(0, BitManipulator.ToInt16(source, 1));
		}
		
		[Test()]
		public void Set_FirstAndLastBitIsSet_LastIsNotSet()
		{
			byte[] destination = {128, 1};
			BitManipulator.Set(destination, 15, false);
			Assert.AreEqual(0, destination[1]);
		}
		
		[Test()]
		public void Set_FirstAndLastBitIsSet_SecondLastSet()
		{
			byte[] destination = {128, 1};
			BitManipulator.Set(destination, 14, true);
			Assert.AreEqual(3, destination[1]);
		}
		
		[Test()]
		public void CountSetBits_ATotalOfFiveBitsIsSet_ReturnsFive()
		{
			byte[] source = {131, 3};
			Assert.AreEqual(5, BitManipulator.CountSetBits(source));
		}
		
		[Test()]
		public void SetRange_NothingIsSet_7To10IsSet()
		{
			byte[] destination = {0, 0};
			
			BitManipulator.SetRange(destination, 6, 5, true);
			
			Assert.AreEqual(3, destination[0]);
			Assert.AreEqual(224, destination[1]);
		}
		
		[Test()]
		public void And_DestinationSetSourceNotSet_BitSet()
		{
			byte[] destination = {128};
			
			BitManipulator.And(destination, 0, false);
			
			Assert.AreEqual(128, destination[0]);
		}
		
		[Test()]
		public void And_DestinationNotSetSourceSet_BitSet()
		{
			byte[] destination = {0};
			
			BitManipulator.And(destination, 0, true);
			
			Assert.AreEqual(128, destination[0]);
		}
		
		[Test()]
		public void And_DestinationAllOddSetSourceAllEvenSet_AllBitsSet()
		{
			byte[] destination = {170, 170};
			byte[] source = {85, 85};
			
			BitManipulator.And(destination, 0, 16, source, 0);
			
			Assert.AreEqual(255, destination[0]);
			Assert.AreEqual(255, destination[1]);
		}
		
		[Test()]
		public void And_DestionationNoneSetSourceLastHalfSet_7To10IsSet()
		{
			byte[] destination = {0, 0};
			byte[] source = {0, 255};
			
			BitManipulator.And(destination, 6, 5, source, 8);
			
			Assert.AreEqual(3, destination[0]);
			Assert.AreEqual(224, destination[1]);
		}
	}
}
