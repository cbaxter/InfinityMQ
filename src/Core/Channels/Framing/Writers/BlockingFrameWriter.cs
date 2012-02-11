using System.Collections.Generic;
using System.IO;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class BlockingFrameWriter : FrameWriterBase
    {
        private readonly Stream stream;

        public BlockingFrameWriter(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.stream = stream;
        }

        public override void Write(IList<Frame> frames)
        {
            WriteFramesToStream(frames, stream);
        }
    }
}
