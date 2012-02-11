using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal class BufferedFrameWriter : FrameWriterBase
    {
        private readonly Int32 maxBufferSize = BufferSize.FromMegabytes(1); //TODO: Make configurable (custom config section setting?)
        private volatile BufferedOutStream activeBufferStream;
        private readonly BufferedOutStream bufferStream1;
        private readonly BufferedOutStream bufferStream2;

        public BufferedFrameWriter(Stream stream)
        {
            Verify.NotNull(stream, "stream");

            this.bufferStream1 = new BufferedOutStream(stream, maxBufferSize);
            this.bufferStream2 = new BufferedOutStream(stream, maxBufferSize);
            this.activeBufferStream = this.bufferStream1;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (!disposing)
                return;

            bufferStream1.Dispose();
            bufferStream2.Dispose(); 
        }

        public override void Write(IList<Frame> frames)
        {
            var totalBytes = frames.Sum(frame => frame.Body.Count);
            if (this.activeBufferStream.Length + totalBytes >= this.maxBufferSize)
            {
                this.activeBufferStream.Flush();
                this.activeBufferStream = ReferenceEquals(this.activeBufferStream, this.bufferStream1) ? this.bufferStream2 : this.bufferStream1;
            }

            WriteFramesToStream(frames, this.activeBufferStream);
        }
    }
}