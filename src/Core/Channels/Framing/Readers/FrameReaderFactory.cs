
using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal interface ICreateFrameReaders
    {
        IReadFrames CreateReader(Stream stream);
    }

    internal class DefaultFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new DefaultFrameReaderFactory();

        public IReadFrames CreateReader(Stream stream)
        {
            return new BlockingFrameReader(stream);
        }
    }

    internal class NullFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new NullFrameReaderFactory();

        public IReadFrames CreateReader(Stream stream)
        {
            return new NullFrameReader();
        }
    }

    internal class BufferedFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new BufferedFrameReaderFactory();

        public IReadFrames CreateReader(Stream stream)
        {
            return new BlockingFrameReader(stream);
        }
    }
}
