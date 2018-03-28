using GZipTest.Model;
using GZipTest.Processing.CompressionModeFactory;
using System;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var parameters = InputParameters.Parse(args);

            using (var finishHandle = new ManualResetEvent(false))
            {
                var factory = CompressionModeFactory.Create(parameters);
                var reader = factory.CreateReader();
                var writer = factory.CreateWriter(finishHandle);

                reader.Run();
                writer.Run();

                using (var compressor = factory.CreateCompressionWorker())
                {
                    compressor.Run();
                }

                finishHandle.WaitOne();
            }

            Console.WriteLine("Completed. Push any key for exit");
            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message);
            Console.WriteLine("Exception. Push any key for exit");
            Console.ReadKey();
            Environment.Exit((e.ExceptionObject as Exception).HResult);
        }
    }
}
