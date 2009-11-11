using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cip.Imaging.SaveHandlers
{
    public class PbmHandler
    {
        public void Save(FastGrayScaleImage img, string path)
        {
            throw new NotImplementedException();
        }
        
        public FastGrayScaleImage Load(string path)
        {
            using (var rs = File.OpenRead(path))
            {
                VerifyFileVersion(rs);

                int width;
                int height;
                int maxColors;
                int bytePerPixel;

                width = ReadInteger(rs);
                height = ReadInteger(rs);
                maxColors = ReadInteger(rs);
                bytePerPixel = maxColors / 8;
            }

            return null;
        }

        private int ReadInteger(FileStream rs)
        {
            int temp = ReadByteAfterWhitespace(rs);
            string buff = "";
            while (temp >= 0 && !IsWhitespace(temp))
                buff += Convert.ToString((byte)temp);
            return int.Parse(buff);
        }

        private static void VerifyFileVersion(FileStream rs)
        {
            byte[] magicNumber = new byte[2];

            if (rs.Read(magicNumber, 0, 2) < 2)
                throw new IOException("Kunde inte lästa filens versions nummer.");

            if (!VerifyMagicNumber(magicNumber))
                throw new IOException("Filens versionsnummer är felaktigt. Måste vara P1 eller P4 men var " + Convert.ToString(magicNumber[0]) + Convert.ToString(magicNumber[1]) + ".");
        }

        private static bool VerifyMagicNumber(byte[] magicNumber)
        {
            return magicNumber[0] != 'P' || (magicNumber[1] != '1' && magicNumber[1] != '4');
        }

        private int ReadByteAfterWhitespace(Stream s)
        {
            int buff = s.ReadByte();
            while (buff >= 0 && IsWhitespace(buff))
                buff = s.ReadByte();
            return buff;
        }

        private static int[] whitespaces = new int[] { 0, 9, 10, 11, 12, 13, 32 };
        private bool IsWhitespace(int c)
        {
            return Array.Exists(whitespaces, x => x == c);
        }
    }
}
