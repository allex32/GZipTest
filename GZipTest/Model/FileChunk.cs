using System;

namespace GZipTest.Model
{
    [Serializable]
    public class FileChunk
    {

        public FileChunk(int id, byte[] content)
        {
            Id = id;
            Content = content;
        }
        public int Id { get; }
        public byte[] Content { get; }
    }
}
