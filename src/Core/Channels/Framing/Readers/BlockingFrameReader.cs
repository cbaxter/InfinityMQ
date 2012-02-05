using System;
using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal class BlockingFrameReader : IReadFrames
    {
        private readonly FrameDemultiplexer frameDemultiplexer;
        private Byte[] streamBuffer;

        public BufferSize BufferSize //TODO: Promote to interface?
        {
            get { return this.streamBuffer.Length; }
            set { this.streamBuffer = new Byte[value]; }
        }

        public BlockingFrameReader()
        {
            this.frameDemultiplexer = new FrameDemultiplexer();
            this.streamBuffer = new Byte[BufferSize.FromKilobytes(64)];
        }

        //TODO: Issue #13 -- Consider custom implementation of FrameReader for Sockets.
        public Frame Read(Stream stream)
        {
            while (this.frameDemultiplexer.FrameCount == 0)
            {
                var availableBytes = stream.Read(this.streamBuffer, 0, this.streamBuffer.Length);

                this.frameDemultiplexer.Write(this.streamBuffer, 0, availableBytes);
            }

            return this.frameDemultiplexer.NextFrame();
        }
    }
}
