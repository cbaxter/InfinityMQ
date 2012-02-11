using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal class BlockingFrameReader : FrameReaderBase
    {
        private readonly Stream stream;

        public BlockingFrameReader(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.stream = stream;
        }

        //TODO: Issue #13 -- Consider custom implementation of FrameReader for Sockets.
        public override Frame ReadFrame()
        {
            return ReadFrameFromStream(this.stream);
        }
    }
}
