using GZipTest.Model;
using GZipTest.Queues;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest.Processing.Decompression
{
    public class FileChunkDecompressionWriter : FileChunkWriter
    {
        public FileChunkDecompressionWriter(string outputFilePath, FileChunkQueue inputQueue, long totalChunkCount, EventWaitHandle finishHandle)
            : base(outputFilePath, inputQueue, totalChunkCount, finishHandle)
        {
        }

        protected override void Write()
        {
            using (var file = new FileStream(OutputFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                FileChunk chunk = null;
                while (InputQueue.TryDequeue(out chunk))
                {
                    file.Write(chunk.Content, 0, chunk.Content.Length);

                    float progress = ((float)chunk.Id / TotalChunkCount);
                    Console.WriteLine(string.Format("{0:P} completed", progress));
                }

                Console.WriteLine("Completed");
                FinishEventHandle.Set();
            }
        }
    }
}
