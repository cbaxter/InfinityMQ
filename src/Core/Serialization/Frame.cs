using System;

namespace InfinityMQ.Serialization
{
    internal class Frame
    {
        public const Int32 PreambleSize = sizeof(Int32) + sizeof(Byte);
        public const Int32 PreambleFlagsOffset = sizeof(Int32);

        private readonly FrameFlags flags;
        private readonly Byte[] body;

        public Int32 Size { get { return this.body.Length; } }
        public FrameFlags Flags { get { return this.flags; } }
        public Byte[] Body { get { return this.body; } }

        public Frame(FrameFlags flags, Byte[] body)
        {
            this.flags = flags;
            this.body = body;
        }
    }
}
