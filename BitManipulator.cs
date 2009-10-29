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
            byte shift = (byte)(index & (byte)7);
            byte value = (byte)(source[index >> 3] & ((byte)128 >> shift));
            return (short)(value << shift);
            //return BitConverter.ToInt16(source, index);
        }

        public static int ToInt32(byte[] source, int index)
        {
            byte shift = (byte)(index & (byte)7);
            byte value = (byte)(source[index >> 3] & ((byte)128 >> shift));
            return (int)(value << shift);
            //return BitConverter.ToInt32(source, index);
        }

        public static bool ToBoolean(byte[] source, int index)
        {
            return BitConverter.ToBoolean(source, index);
        }

        public static void Set(byte[] destination, int index, int value)
        {
            Set(destination, index, value != 0);
        }

        public static void Set(byte[] destination, int index, bool value)
        {
            if (value)
            {
                //Debug.Write(string.Format("Set: Index={0};", index));
                //Debug.Write(string.Format("Pos={0};", index >> 3));
                //Debug.WriteLine(string.Format("Mask={0};", (byte)((byte)128 >> (index & (byte)7))));
                destination[index >> 3] |= (byte)((byte)128 >> (index & (byte)7));
            }
            else
            {
                //Debug.Write(string.Format("Unset: Index={0};", index));
                //Debug.Write(string.Format("Pos={0};", index >> 3));
                //Debug.WriteLine(string.Format("Mask={0};", ~(byte)((byte)128 >> (byte)(index & 7))));
                destination[index >> 3] &= (byte)(~(byte)128 >> (byte)(index & 7));
            }
        }
    }
}
