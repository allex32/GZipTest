using GZipTest.Queues;
using System.IO;

namespace GZipTest.Processing.Compression
{
    public class FileChunkCompressionReader : FileChunkReader
    {
        public int ChunkByteSize { get; }
        public FileChunkCompressionReader(string inputFilePath, FileChunkQueue processingQueue,
            int chunkByteSize = 2 << 23) : base(inputFilePath, processingQueue)
        { 
            ChunkByteSize = chunkByteSize;
        }

        public override long TotalChunkCount
        {
            get
            {
                var fileInfo = new FileInfo(InputFilePath);
                return fileInfo.Length % ChunkByteSize == 0
                    ? (fileInfo.Length / ChunkByteSize)
                    : (fileInfo.Length / ChunkByteSize) + 1;
            }
        }
        protected override void Read()
        {          
            var buffer = new byte[ChunkByteSize];
            var chunkCounter = 1;
          
            using (var file = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read))
            {
                int numBytesRead = file.Read(buffer, 0, ChunkByteSize);
                    
                while (numBytesRead > 0)
                {
                    ProcessingQueue.Enqueue(chunkCounter++, buffer);
                    numBytesRead = file.Read(buffer, 0, buffer.Length);
                }
            }
            ProcessingQueue.Close();
        }
    }
}
