using GZipTest.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest.Processing
{
    public abstract class FileChunkReader
    {
        private Thread _worker;
        public string InputFilePath { get; }
        protected FileChunkQueue ProcessingQueue { get; }

        public abstract long TotalChunkCount { get; } 

        public FileChunkReader(string inputFilePath, FileChunkQueue processingQueue)
        {
            InputFilePath = inputFilePath;
            ProcessingQueue = processingQueue;

            _worker = new Thread(Read);
        }

        public void Run()
        {
            _worker.Start();
        }

        protected abstract void Read();
    }
}
