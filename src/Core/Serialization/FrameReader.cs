using System;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class FrameReader
    {
        private const Int32 BufferSize = 8192;
        private readonly FrameDemultiplexer frameDemultiplexer;
        private readonly Byte[] streamBuffer;
        private readonly Stream baseStream;

        public FrameReader(Stream stream)
            : this(stream, BufferSize)
        { }

        public FrameReader(Stream stream, Int32 bufferSize)
        {
            Verify.NotNull(stream, "stream");

            this.baseStream = stream;
            this.streamBuffer = new Byte[bufferSize];
            this.frameDemultiplexer = new FrameDemultiplexer();
        }

        //TODO: Issue #13 -- Consider custom implementation of FrameReader for Sockets.
        public Frame Read()
        {
            while (frameDemultiplexer.FrameCount == 0)
            {
                var availableBytes = this.baseStream.Read(this.streamBuffer, 0, this.streamBuffer.Length);
                
                frameDemultiplexer.Write(this.streamBuffer, 0, availableBytes);
            }

            return frameDemultiplexer.NextFrame();
        }
    }
}