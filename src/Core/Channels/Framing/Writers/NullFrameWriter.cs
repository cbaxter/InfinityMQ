using System;
using System.Collections.Generic;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class NullFrameWriter : FrameWriterBase
    {
        public override void Write(IList<Frame> frames)
        {
            throw new NotSupportedException(ExceptionMessages.EndpointWriteNotSupported);
        }
    }
}
