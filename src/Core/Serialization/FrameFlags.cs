using System;

namespace InfinityMQ.Serialization
{
    [Flags]
    internal enum FrameFlags : sbyte
    {
        None = 0x00,
        More = 0x01,
        Header = 0x02,
        //Reserved = 0x04,
        //Reserved = 0x08,
        //Reserved = 0x10,
        //Reserved = 0x20,
        //Reserved = 0x40
    }
}
