using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cip.Imaging.Tool
{
    public static class BitManipulator
    {
        public static short ToInt16(byte[] source, int index)
        {
            return (short)(ToBoolean(source, index) ? 1 : 0);
        }

        public static int ToInt32(byte[] source, int index)
        {
            return ToBoolean(source, index) ? 1 : 0;
        }

        public static bool ToBoolean(byte[] source, int index)
        {
            byte shift = (byte)(index & (byte)7);
            byte value = (byte)(source[index >> 3] & ((byte)128 >> shift));
            return value != 0;
        }

        public static void Set(byte[] destination, int index, int value)
        {
            Set(destination, index, value != 0);
        }

        public static void Set(byte[] destination, int index, bool value)
        {
            if (value)
                destination[index >> 3] |= (byte)((byte)128 >> (index & (byte)7));
            else
                destination[index >> 3] &= (byte)(~(byte)128 >> (byte)(index & 7));
        }

        public static void SetRange(byte[] destination, int index, int count, bool value)
        {
            //TODO: Optimera det här lite... 
            //(Kanske inte behövs, kostnaden för att optimera blir troligen större än vinsten om det sällan är höga värden på count)
            for (int i = index; i < index + count; i++)
                Set(destination, i, value);
        }

        public static int CountSetBits(byte source)
        {
            // Algoritmen kommer från: http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
            int v = (int)source; // count the number of bits set in v
            int c; // c accumulates the total bits set in v
            for (c = 0; v != 0; c++)
                v &= v - 1; // clear the least significant bit set
            return c;
        }

        public static int CountSetBits(byte[] source)
        {
            return CountSetBits(source, 0, source.Length);
        }

        public static int CountSetBits(byte[] source, int startIndex, int count)
        {
            int c = 0;
            int index = startIndex;
            while (index < count + startIndex)
                c += CountSetBits(source[index]);
            return c;
        }

		public static byte[] And(byte[] a, byte[] b)
		{
			if(a.Length != b.Length)
				throw new IndexOutOfRangeException("Byte arrays must be of same lenght.");
			
			byte[] result = new byte[a.Length];
			
			for (int i = 0; i < a.Length; i++) 
				result[i] = (byte)(a[i] & b[i]);
			
			return result;
		}
    }
}
