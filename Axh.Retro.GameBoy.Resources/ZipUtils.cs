using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace Axh.Retro.GameBoy
{
    /// <summary>
    /// Utils for zipped bytes.
    /// </summary>
    public static class ZipUtils
    {
        /// <summary>
        /// Unzips the specified bytes. TODO: this is crude.
        /// </summary>
        /// <param name="compressedBytes">The compressed bytes.</param>
        /// <returns></returns>
        public static byte[] UnZip(this byte[] compressedBytes)
        {
            using (var stream = new MemoryStream(compressedBytes))
            using (var zipFile = new ZipFile(stream))
            {
                var zipEntry = zipFile.Cast<ZipEntry>().FirstOrDefault(x => x.IsFile);
                using (var zipStream = zipFile.GetInputStream(zipEntry))
                using (var outStream = new MemoryStream())
                {
                    zipStream.CopyTo(outStream, 4096);
                    return outStream.ToArray();
                }
            }
        }
    }
}