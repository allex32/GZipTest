using GZipTest.Model;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Processing.Compression
{
    public class GzCompressionStrategy : ICompressionModeStrategy
    {
        public FileChunk Compress(FileChunk inputChunk)
        {       
            using(var outputStream = new MemoryStream())
            {
                using(var compressionStream = new GZipStream(outputStream, CompressionMode.Compress, true))
                {
                    compressionStream.Write(inputChunk.Content, 0, inputChunk.Content.Length);
                }
                return new FileChunk(inputChunk.Id, outputStream.ToArray());
            }
        }
    }
}
