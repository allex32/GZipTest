using GZipTest.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest.Queues
{
    public class FileChunkQueue 
        //: IDisposable
         
    {
        #region Static 
        readonly static int MAX_QUEUE_SIZE = 20;
        #endregion

        #region Members
        readonly object _locker = new object();

        Queue<FileChunk> _chunkQueue = new Queue<FileChunk>(MAX_QUEUE_SIZE);
        int _nextChunkOrderNumber = 1;
        bool _closed = false;
        #endregion

        public void Close()
        {
            lock(_locker)
            {
                _closed = true;
                Monitor.PulseAll(_locker);
            }
        }
        public void Enqueue(int chunkId, byte[] buffer)
        {
            var bufferCopy = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, bufferCopy, 0, buffer.Length);

            Enqueue(new FileChunk(chunkId, bufferCopy));
        }
        public void Enqueue(FileChunk chunk)
        {
            lock (_locker)
            {
                if (_closed)
                    throw new InvalidOperationException("Queue is closed");

                while(_chunkQueue.Count >= MAX_QUEUE_SIZE 
                    || chunk.Id != _nextChunkOrderNumber)
                {
                    Monitor.Wait(_locker);
                }

                _chunkQueue.Enqueue(chunk);
                ++_nextChunkOrderNumber;

                Monitor.PulseAll(_locker);          
            }
        }
        public bool TryDequeue(out FileChunk outChunk)
        {
            lock (_locker)
            {
                while (_chunkQueue.Count == 0)
                {
                    if (_closed)
                    {
                        outChunk = null;
                        return false;
                    }
                    Monitor.Wait(_locker);
                }
                outChunk = _chunkQueue.Dequeue();
                Monitor.PulseAll(_locker);
            }

            return true;
        }
    }
}
