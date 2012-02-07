using System.Collections.Generic;
using System.IO;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal interface IWriteFrames
    {
        void Write(Stream stream, IList<Frame> frames);
    }
}
