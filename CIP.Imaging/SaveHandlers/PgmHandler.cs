using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cip.Imaging.SaveHandlers
{
    public class PgmHandler
    {
        public void Save(FastGrayScaleImage img, string path)
        {
            using (var rs = File.OpenWrite(path))
            {
                string header = string.Format("P5 {0} {1} {2} ", img.Width, img.Height, 255);
                byte[] rawHeader = System.Text.Encoding.ASCII.GetBytes(header);
                rs.Write(rawHeader, 0, rawHeader.Length);
                rs.Write(img.Bits, 0, img.Width * img.Height);
                rs.Close();
            }
        }
        
        /// <summary>
        /// Det här är en väldigt enkel implementation BPM. Den stödjer som mest 256 nivåers gråskala. Saknar även stöd för kommentarer i filen.
        /// </summary>
        /// <param name="path">Sökväg till bild att öppna.</param>
        public FastGrayScaleImage Load(string path)
        {
            FastGrayScaleImage result = null;

            using (var rs = File.OpenRead(path))
            {
                VerifyFileVersion(rs);

                int width;
                int height;
                int maxColors;
 
                width = ReadInteger(rs);
                height = ReadInteger(rs);
                maxColors = ReadInteger(rs);
                
                if (maxColors > 255)
                    throw new NotSupportedException("Den heär implementationen stödjer som mest 256 nivåers gråskala");

                result = new FastGrayScaleImage(width, height);

                int readCount = rs.Read(result.Bits, 0, width*height);

                if(readCount != width * height)
                    System.Diagnostics.Trace.WriteLine(string.Format("Inläst antal bytes motsvarar inte bildens storlek. Borde vara {0} men blev {1}.", width * height, readCount));
                
                rs.Close();
            }

            return result;
        }

        private int ReadInteger(FileStream rs)
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

        private static void VerifyFileVersion(FileStream rs)
        {
            byte[] magicNumber = new byte[2];

            if (rs.Read(magicNumber, 0, 2) < 2)
                throw new IOException("Kunde inte lästa filens versions nummer.");

            if (!VerifyMagicNumber(magicNumber))
                throw new IOException("Filens versionsnummer är felaktigt. Måste vara P5 men var " + Encoding.ASCII.GetString(magicNumber) + ".");
        }

        private static bool VerifyMagicNumber(byte[] magicNumber)
        {
            return Encoding.ASCII.GetString(magicNumber) == "P5";
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
