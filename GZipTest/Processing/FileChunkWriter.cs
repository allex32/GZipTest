using GZipTest.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest.Processing
{
    public abstract class FileChunkWriter
    {
        private Thread _worker;
        public string OutputFilePath { get; }
        public long TotalChunkCount { get; }
        protected FileChunkQueue InputQueue { get; }
        
        protected EventWaitHandle FinishEventHandle { get; }


        public FileChunkWriter(string outputFilePath, FileChunkQueue inputQueue, long totalChunkCount, EventWaitHandle finishHandle)
        {
            OutputFilePath = outputFilePath;
            InputQueue = inputQueue;
            TotalChunkCount = totalChunkCount;

            FinishEventHandle = finishHandle;
            _worker = new Thread(Write);
        }

        public void Run()
        {
            _worker.Start();
        }
        protected abstract void Write();
    }
}
