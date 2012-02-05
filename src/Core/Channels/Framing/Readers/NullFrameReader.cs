using System;
using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal class NullFrameReader : IReadFrames
    {
        public Frame Read(Stream stream)
        {
            throw new NotSupportedException(); //TODO: Add meaninful message.
        }
    }
}
