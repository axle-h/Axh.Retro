namespace Axh.Retro.GameBoy.Tests.Util
{
    using System.IO;
    using System.Linq;
    
    using ICSharpCode.SharpZipLib.Zip;

    internal static class ZipUtils
    {
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
