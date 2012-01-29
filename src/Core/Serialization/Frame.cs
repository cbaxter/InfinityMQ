using System;

namespace InfinityMQ.Serialization
{
    internal class Frame
    {
        public const Int32 PreambleSize = sizeof(Int32) + sizeof(Byte);
        public const Int32 PreambleFlagsOffset = sizeof(Int32);

        private readonly ArraySegment<Byte> body;
        private readonly FrameFlags flags;

        public ArraySegment<Byte> Body { get { return this.body; } }
        public FrameFlags Flags { get { return this.flags; } }

        public Frame(Byte[] raw)
            : this((FrameFlags)raw[0], new ArraySegment<Byte>(raw, 1, raw.Length - 1))
        { }

        public Frame(FrameFlags flags, ArraySegment<Byte> body)
        {
            this.body = body;
            this.flags = flags;
        }
    }
}