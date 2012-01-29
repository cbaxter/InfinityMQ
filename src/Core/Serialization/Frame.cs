using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class Frame
    {
        public const Int32 PreambleSize = sizeof(Int32) + sizeof(Byte);

        private readonly FrameFlags flags;
        private readonly ArraySegment<Byte> body;

        public Int32 Size { get { return this.body.Count; } }
        public FrameFlags Flags { get { return this.flags; } }
        public ArraySegment<Byte> Body { get { return this.body; } }

        public Frame(FrameFlags flags, Byte[] body)
            : this(flags, new ArraySegment<Byte>(body, 0, body.Length))
        { }

        public Frame(FrameFlags flags, ArraySegment<Byte> body)
        {
            this.flags = flags;
            this.body = body;
        }

        public Stream ToStream()
        {
            return new MemoryStream(body.Array, body.Offset, body.Count);
        }
    }
}
