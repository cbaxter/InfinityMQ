using System;
using System.Collections.Generic;
using System.IO;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class NullFrameWriter : IWriteFrames
    {
        public void Write(Stream stream, IList<Frame> frames)
        {
            throw new NotSupportedException(); //TODO: Add meaninful message.
        }
    }
}
