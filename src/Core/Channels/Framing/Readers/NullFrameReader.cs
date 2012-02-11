using System;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal class NullFrameReader : FrameReaderBase
    {
        public override Frame ReadFrame()
        {
            throw new NotSupportedException(); //TODO: Issue #23 - Throw meaningful execptions.
        }
    }
}
