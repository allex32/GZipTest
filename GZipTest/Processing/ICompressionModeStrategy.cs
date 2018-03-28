using GZipTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.Processing
{
    public interface ICompressionModeStrategy
    {
        FileChunk Compress(FileChunk input);
    }
}
