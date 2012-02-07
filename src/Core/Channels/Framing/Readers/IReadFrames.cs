using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal interface IReadFrames
    {
        Frame Read(Stream stream);
    }
}
