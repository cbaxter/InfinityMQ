using System;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal interface IReadFrames : IDisposable
    {
        Frame ReadFrame();
    }
}
