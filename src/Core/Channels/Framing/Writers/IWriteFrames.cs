using System;
using System.Collections.Generic;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal interface IWriteFrames : IDisposable
    {
        void Write(IList<Frame> frames);
    }
}
