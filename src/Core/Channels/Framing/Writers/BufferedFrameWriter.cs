using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class BufferedFrameWriter : IWriteFrames
    {
        private readonly Int32 maxBufferSize = BufferSize.FromMegabytes(1); //TODO: Make configurable (custom config section setting?)
        private volatile BufferedOutStream activeBufferStream;
        private readonly BufferedOutStream bufferStream1;
        private readonly BufferedOutStream bufferStream2;
        private readonly IWriteFrames frameWriter;

        public BufferedFrameWriter()
        {
            this.bufferStream1 = new BufferedOutStream(maxBufferSize);
            this.bufferStream2 = new BufferedOutStream(maxBufferSize);
            this.frameWriter = new BlockingFrameWriter();
            this.activeBufferStream = this.bufferStream1;
        }

        public void Write(Stream stream, IList<Frame> frames)
        {
            this.activeBufferStream.EnsureDelegateStream(stream);

            var totalBytes = frames.Sum(frame => frame.Body.Count);
            if (this.activeBufferStream.Length + totalBytes >= this.maxBufferSize)
            {
                this.activeBufferStream.Flush();
                this.activeBufferStream = ReferenceEquals(this.activeBufferStream, this.bufferStream1) ? this.bufferStream2 : this.bufferStream1;
            }

            this.frameWriter.Write(this.activeBufferStream, frames);
        }
    }
}