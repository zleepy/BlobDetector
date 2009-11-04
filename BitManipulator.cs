using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Histogram
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
            for (int i = index; i < index + count; i++)
                Set(destination, i, value);
        }
    }
}
