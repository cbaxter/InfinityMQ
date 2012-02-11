using System.IO;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal interface ICreateFrameWriters
    {
        IWriteFrames CreateWriter(Stream stream);
    }

    internal class DefaultFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new DefaultFrameWriterFactory();

        public IWriteFrames CreateWriter(Stream stream)
        {
            return new BlockingFrameWriter(stream);
        }
    }

    internal class NullFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new NullFrameWriterFactory();

        public IWriteFrames CreateWriter(Stream stream)
        {
            return new NullFrameWriter();
        }
    }

    internal class BufferedFrameWriterFactory : ICreateFrameWriters
    {
        public static readonly ICreateFrameWriters Instance = new BufferedFrameWriterFactory();

        public IWriteFrames CreateWriter(Stream stream)
        {
            return new BufferedFrameWriter(stream);
        }
    }
}
