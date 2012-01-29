using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class FrameReader
    {
        private readonly Stream baseStream;
        private readonly Byte[] lengthBuffer;

        public FrameReader(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.baseStream = stream;
            this.lengthBuffer = new Byte[sizeof(Int32)];
        }

        public Frame Read()
        {
            var bytesConsumed = 0;
            var buffer = lengthBuffer;
            var length = sizeof(Int32);
            var lengthKnown = false;

            do
            {
                bytesConsumed += this.baseStream.Read(buffer, bytesConsumed, length - bytesConsumed);

                if (lengthKnown || bytesConsumed != length) 
                    continue;

                length = BitConverter.ToInt32(buffer, 0);
                buffer = new Byte[length];
                lengthKnown = true;
                bytesConsumed = 0;
            } while (bytesConsumed < length);

            return length < 1 ? null : new Frame((FrameFlags)buffer[0], buffer.ToSegment(1));
        }
    }
}
