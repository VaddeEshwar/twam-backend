using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgmtDAL.Helpers
{
    public static class CompressData
    {
        public static byte[] CompressString(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The string cannot be null or empty.", nameof(text));

            byte[] bytes = Encoding.UTF8.GetBytes(text);

            using (var outputStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                }

                return outputStream.ToArray();
            }
        }
    }
}
