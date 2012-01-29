using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class FrameWriter
    {
        private readonly Stream baseStream;

        public Stream BaseStream { get { return this.baseStream; } }

        public FrameWriter(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.baseStream = stream;
        }

        public void Write(Frame frame)
        {
            if (frame == null)
                return;

            this.baseStream.Write(BitConverter.GetBytes(sizeof(Byte) + frame.Size));
            this.baseStream.WriteByte((Byte)frame.Flags);
            this.baseStream.Write(frame.Body);
        }
    }
}
