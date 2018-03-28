using GZipTest.Model;
using GZipTest.Processing.Compression;
using GZipTest.Processing.Decompression;
using GZipTest.Queues;
using System;
using System.Threading;

namespace GZipTest.Processing.CompressionModeFactory
{
    public abstract class CompressionModeFactory
    {
        #region Properties
        protected string InputFilePath { get;}
        protected string OutputFilePath { get; }       
        protected FileChunkQueue CompressionQueue { get; }
        protected FileChunkQueue WritingQueue { get; }

        #endregion

        public CompressionModeFactory(string inputFilePath, string outputFilePath,
            FileChunkQueue compressionQueue, FileChunkQueue writingQueue)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            CompressionQueue = compressionQueue;
            WritingQueue = writingQueue;
        }
        public abstract FileChunkReader CreateReader();
        public abstract FileChunkWriter CreateWriter(EventWaitHandle finishHandle);

        public CompressionWorker CreateCompressionWorker()
        {
            return new CompressionWorker(CompressionQueue, WritingQueue, CreateCompressionModeStrategy());
        }
        protected abstract ICompressionModeStrategy CreateCompressionModeStrategy();
        public static CompressionModeFactory Create(InputParameters parameters)
        {
            var compressionQueue = new FileChunkQueue();
            var writingQueue = new FileChunkQueue();

            if (String.Equals(parameters.CompressionMode, "compress", StringComparison.InvariantCultureIgnoreCase))
                return new GzCompressionFactory(parameters.InputFilePath, parameters.OutputFilePath,
                    compressionQueue, writingQueue);

            if (String.Equals(parameters.CompressionMode, "decompress", StringComparison.InvariantCultureIgnoreCase))
                return new GzDecompressionFactory(parameters.InputFilePath, parameters.OutputFilePath,
                    compressionQueue, writingQueue);

            throw new ArgumentOutOfRangeException("First parameter must have 'compress' or 'decompress' values");
        }

    }

    public class GzCompressionFactory : CompressionModeFactory
    {
        public GzCompressionFactory(string inputFilePath, string outputFilePath, 
            FileChunkQueue processingQueue, FileChunkQueue writingQueue) 
            : base(inputFilePath, outputFilePath, processingQueue, writingQueue)
        {
        }

        protected override ICompressionModeStrategy CreateCompressionModeStrategy()
        {
            return new GzCompressionStrategy();
        }

        public override FileChunkReader CreateReader()
        {
            return new FileChunkCompressionReader(InputFilePath, CompressionQueue);
        }

        public override FileChunkWriter CreateWriter(EventWaitHandle finishHandle)
        {
            var totalChunkCount = CreateReader().TotalChunkCount;

            return new FileChunkCompressionWriter(OutputFilePath, WritingQueue, totalChunkCount, finishHandle);
        }
    }

    public class GzDecompressionFactory : CompressionModeFactory
    {
        public GzDecompressionFactory(string inputFilePath, string outputFilePath,
            FileChunkQueue processingQueue, FileChunkQueue writingQueue) 
            : base(inputFilePath, outputFilePath, processingQueue, writingQueue)
        {
        }

        public override FileChunkReader CreateReader()
        {
            return new FileChunkDecompressionReader(InputFilePath, CompressionQueue);
        }

        public override FileChunkWriter CreateWriter(EventWaitHandle finishHandle)
        {
            var totalChunkCount = CreateReader().TotalChunkCount;
            return new FileChunkDecompressionWriter(OutputFilePath, WritingQueue, totalChunkCount, finishHandle);
        }

        protected override ICompressionModeStrategy CreateCompressionModeStrategy()
        {
            return new GzDecompressionStrategy();
        }
    }
}
