using System;
using System.IO;

namespace InfinityMQ.Channels.Framing.Readers
{
    internal abstract class FrameReaderBase : IReadFrames
    {
        private readonly FrameDemultiplexer frameDemultiplexer;
        private readonly Byte[] streamBuffer;

        protected FrameReaderBase()
        {
            this.frameDemultiplexer = new FrameDemultiplexer();
            this.streamBuffer = new Byte[BufferSize.FromKilobytes(64)]; //TODO: Issue #18 - Option Configuration.
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        { }

        public abstract Frame ReadFrame();

        //TODO: Issue #13 -- Consider custom implementation of FrameReader for Sockets.
        protected Frame ReadFrameFromStream(Stream stream)
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
