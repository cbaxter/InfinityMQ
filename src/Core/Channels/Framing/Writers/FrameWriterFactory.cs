
namespace InfinityMQ.Channels.Framing.Writers
{
    internal interface ICreateFrameWriters
    {
        IWriteFrames CreateWriter();
    }

    internal class DefaultFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new DefaultFrameWriterFactory();

        public IWriteFrames CreateWriter()
        {
            return new BlockingFrameWriter();
        }
    }

    internal class NullFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new NullFrameWriterFactory();

        public IWriteFrames CreateWriter()
        {
            return new NullFrameWriter();
        }
    }

    internal class BufferedFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new BufferedFrameWriterFactory();

        public IWriteFrames CreateWriter()
        {
            return new BlockingFrameWriter(); //TODO: Create BufferedFrameWriter.
        }
    }
}
