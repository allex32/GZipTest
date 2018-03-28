using System;
using System.IO;
using System.Linq;

namespace GZipTest.Model
{
    public class InputParameters
    {
        public string CompressionMode { get; }
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        private InputParameters(string compressionMode, string inputFilePath, string outputFilePath)
        {
            CompressionMode = compressionMode;
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        /// <summary>
        /// Валидирует массив параметров и переводит в структурированный вид
        /// </summary>
        /// <param name="pars">
        /// Массив из трех аргументов:
        /// 1) compress/decompress; 2) путь до целевого файла; 3) путь до выходного файла
        /// </param>
        /// <returns></returns>
        public static InputParameters Parse(params string[] pars)
        {
            if (pars.Count() != 3)
                throw new ArgumentException("Exactly 3 input arguments are expected");

            var compressionMode = pars[0];

            if (!(String.Equals(compressionMode, "decompress", StringComparison.InvariantCultureIgnoreCase)
                || String.Equals(compressionMode, "compress", StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException("First parameter must have 'compress' or 'decompress' values");


            var inputFilePath = Path.GetFullPath(pars[1]);
            var outputFilePath = Path.GetFullPath(pars[2]);

            return new InputParameters(compressionMode, inputFilePath, outputFilePath);
        }
    }
}
