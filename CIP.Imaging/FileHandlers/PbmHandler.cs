using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cip.Imaging.FileHandlers
{
    class PbmHandler : PnmBase
    {
        public void Save(byte[] img, int width, int height, string path)
        {
            using (var rs = File.OpenWrite(path))
            {
                string header = string.Format("P4 {0} {1} ", width, height);
                byte[] rawHeader = System.Text.Encoding.ASCII.GetBytes(header);
                rs.Write(rawHeader, 0, rawHeader.Length);
                rs.Write(img, 0, width * height);
                rs.Close();
            }
        }

        /// <summary>
        /// Det här är en väldigt enkel implementation BPM. Den stödjer som mest 256 nivåers gråskala. Saknar även stöd för kommentarer i filen.
        /// </summary>
        /// <param name="path">Sökväg till bild att öppna.</param>
        public byte[] Load(string path, out int width, out int height)
        {
            byte[] result = null;

            using (var rs = File.OpenRead(path))
            {
                VerifyFileVersion(rs, "P4");

                width = ReadInteger(rs);
                height = ReadInteger(rs);
                int byteCount = (int)Math.Ceiling(width * height / 8.0);

                result = new byte[byteCount];

                int readCount = rs.Read(result, 0, byteCount);

                if (readCount != byteCount)
                    System.Diagnostics.Trace.WriteLine(string.Format("Inläst antal bytes motsvarar inte bildens storlek. Borde vara {0} men blev {1}.", byteCount, readCount));

                rs.Close();
            }

            return result;
        }
    }
}
