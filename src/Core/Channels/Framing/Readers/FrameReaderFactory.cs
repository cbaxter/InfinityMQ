
namespace InfinityMQ.Channels.Framing.Readers
{
    internal interface ICreateFrameReaders
    {
        IReadFrames CreateReader();
    }

    internal class DefaultFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new DefaultFrameReaderFactory();

        public IReadFrames CreateReader()
        {
            return new BlockingFrameReader();
        }
    }

    internal class NullFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new NullFrameReaderFactory();

        public IReadFrames CreateReader()
        {
            return new NullFrameReader();
        }
    }

    internal class BufferedFrameReaderFactory : ICreateFrameReaders
    {
        public static readonly ICreateFrameReaders Instance = new BufferedFrameReaderFactory();

        public IReadFrames CreateReader()
        {
            return new BlockingFrameReader();  //TODO: Issue #21 - Implement BufferedFrameReader/BufferedFrameWriter.
        }
    }
}
