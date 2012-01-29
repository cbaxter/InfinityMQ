using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class FrameWriter
    {
        private readonly Byte[] preamble = new Byte[Frame.PreambleSize];
        private readonly Stream baseStream;

        public Stream BaseStream { get { return this.baseStream; } }

        public FrameWriter(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.baseStream = stream;
        }

        public void Write(Frame frame)
        {
            Write(frame.Body, 0, frame.Body.Length, frame.Flags);
        }

        public void Write(Byte[] buffer, Int32 offset, Int32 count, FrameFlags flags)
        {
            unsafe
            {
                fixed (Byte* numRef = preamble)
                    *((Int32*)numRef) = sizeof(Byte) + count;

                preamble[Frame.PreambleFlagsOffset] = (Byte)flags;
            }

            this.baseStream.Write(preamble, 0, preamble.Length);
            this.baseStream.Write(buffer, offset, count);
        }
    }
}
