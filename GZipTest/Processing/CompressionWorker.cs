using GZipTest.Model;
using GZipTest.Queues;
using System;
using System.Threading;

namespace GZipTest.Processing
{
    public class CompressionWorker : IDisposable
    {
        #region Members
        bool disposed = false;

        static int _numOfWorkerThreads = (Environment.ProcessorCount - 2) > 0
            ? Environment.ProcessorCount - 2
            : 1;


        Thread[] _workers = new Thread[_numOfWorkerThreads];
        WaitHandle[] _waitHandles = new WaitHandle[_numOfWorkerThreads];
        #endregion

        public ICompressionModeStrategy CompressionStrategy { get; }
        private FileChunkQueue InputQueue { get; }
        private FileChunkQueue OutputQueue { get; }
        
        public CompressionWorker(FileChunkQueue inputQueue, FileChunkQueue outputQueue,
            ICompressionModeStrategy compressionStrategy)
        {
            InputQueue = inputQueue;
            OutputQueue = outputQueue;
            CompressionStrategy = compressionStrategy;

            for (int i = 0; i < _numOfWorkerThreads; ++i)
            {
                _waitHandles[i] = new AutoResetEvent(false);
                _workers[i] = new Thread(new ParameterizedThreadStart(RunCompression));
            }           
        }

        public void Run()
        {
            for (int i = 0; i < _numOfWorkerThreads; ++i)
                _workers[i].Start(_waitHandles[i]);

            try
            {
                WaitHandle.WaitAll(_waitHandles);
            }           
            finally
            {
               OutputQueue.Close();
            }
            
        }

        private void RunCompression(object ewh)
        {
            EventWaitHandle eventWaitHandle = ewh as EventWaitHandle;
            if (eventWaitHandle == null) 
                throw new ArgumentException($"{nameof(eventWaitHandle)} has incorrect type or is null");
            
        
            FileChunk taskChunk;
            while(InputQueue.TryDequeue(out taskChunk))
            {
                var result = CompressionStrategy.Compress(taskChunk);
                OutputQueue.Enqueue(result);
            }
            eventWaitHandle.Set();

        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                OutputQueue.Close();
                foreach (var waitHandle in _waitHandles)
                    waitHandle?.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}
