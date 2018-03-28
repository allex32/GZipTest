using GZipTest.Model;
using GZipTest.Queues;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest.Processing.Decompression
{
    public class FileChunkDecompressionReader : FileChunkReader
    {
        private long _totalChunkCount = -1;

        public FileChunkDecompressionReader(string inputFilePath, FileChunkQueue processingQueue) 
            : base(inputFilePath, processingQueue)
        {
        }

        public override long TotalChunkCount
        {
            get
            {
                if (_totalChunkCount == -1)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (var file = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read))
                    {
                        _totalChunkCount = (long)formatter.Deserialize(file);
                    }
                }

                return _totalChunkCount;
            }
        }

        protected override void Read()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (var file = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read))
            {
                var totalChunkCount = (long)formatter.Deserialize(file);
                while (file.Position < file.Length)
                {
                    var chunk = (FileChunk)formatter.Deserialize(file);
                    ProcessingQueue.Enqueue(chunk);
                }
            }
            ProcessingQueue.Close();
        }
    }
}
