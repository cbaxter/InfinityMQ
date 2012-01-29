﻿using System;
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
            var lengthKnown = false;
            var buffer = lengthBuffer;
            var length = sizeof(Int32);
            var flags = FrameFlags.None;

            do
            {
                bytesConsumed += this.baseStream.Read(buffer, bytesConsumed, length - bytesConsumed);

                if (lengthKnown || bytesConsumed != length)
                    continue;

                length = BitConverter.ToInt32(buffer, 0) - sizeof(Byte);
                flags = (FrameFlags)baseStream.ReadByte();
                buffer = new Byte[length];
                lengthKnown = true;
                bytesConsumed = 0;
            } while (length != 0 && bytesConsumed < length);

            return new Frame(flags, buffer);
        }
    }
}
