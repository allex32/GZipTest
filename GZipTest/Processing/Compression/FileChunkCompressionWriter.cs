using GZipTest.Model;
using GZipTest.Queues;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace GZipTest.Processing
{
    public class FileChunkCompressionWriter : FileChunkWriter
    {
        public FileChunkCompressionWriter(string outputFilePath, FileChunkQueue inputQueue, long totalChunkCount, EventWaitHandle finishHandle)
            :base(outputFilePath, inputQueue, totalChunkCount, finishHandle)
        {
        }
        
        protected override void Write()
        {        
            using (var file = new FileStream(OutputFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, TotalChunkCount);

                FileChunk chunk = null;                   
                while (InputQueue.TryDequeue(out chunk))
                {
                    formatter.Serialize(file, chunk);

                    float progress = ((float)chunk.Id / TotalChunkCount);
                    Console.WriteLine(string.Format("{0:P} completed", progress));
                }

                Console.WriteLine("Completed");
                FinishEventHandle.Set();
            } 
        }        
    }
}
