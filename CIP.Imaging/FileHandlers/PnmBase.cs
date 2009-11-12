using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cip.Imaging.FileHandlers
{
    public abstract class PnmBase
    {
        protected int ReadInteger(FileStream rs)
        {
            int temp = ReadByteAfterWhitespace(rs);
            string buff = "";
            while (temp >= 0 && !IsWhitespace(temp))
            {
                buff += (char)temp;
                temp = rs.ReadByte();
            }
            return int.Parse(buff);
        }

        protected static void VerifyFileVersion(Stream rs, string validMagicNumber)
        {
            byte[] magicNumber = new byte[2];

            if (rs.Read(magicNumber, 0, 2) < 2)
                throw new IOException("Kunde inte lästa filens versions nummer.");

            if (Encoding.ASCII.GetString(magicNumber) != validMagicNumber)
                throw new IOException("Filens versionsnummer är felaktigt. Måste vara " + validMagicNumber + " men var " + Encoding.ASCII.GetString(magicNumber) + ".");
        }

        protected int ReadByteAfterWhitespace(Stream s)
        {
            int buff = s.ReadByte();
            while (buff >= 0 && IsWhitespace(buff))
                buff = s.ReadByte();
            return buff;
        }

        private static int[] whitespaces = new int[] { 0, 9, 10, 11, 12, 13, 32 };
        protected bool IsWhitespace(int c)
        {
            return Array.Exists(whitespaces, x => x == c);
        }
    }
}
