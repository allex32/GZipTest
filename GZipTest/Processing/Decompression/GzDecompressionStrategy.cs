using System.IO;
using System.IO.Compression;
using GZipTest.Model;

namespace GZipTest.Processing.Decompression
{
    public class GzDecompressionStrategy : ICompressionModeStrategy
    {
        public FileChunk Compress(FileChunk inputChunk)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var decompressionStream = new GZipStream(new MemoryStream(inputChunk.Content), CompressionMode.Decompress, false))
                {                               
                    byte[] buffer = new byte[inputChunk.Content.Length];
                    int bytesRead = decompressionStream.Read(buffer, 0, buffer.Length);
                    while (bytesRead > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                        bytesRead = decompressionStream.Read(buffer, 0, buffer.Length);
                    }                                    
                }
                return new FileChunk(inputChunk.Id, outputStream.ToArray());
            }
        }
        
    }
}
