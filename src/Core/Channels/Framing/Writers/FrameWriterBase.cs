using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfinityMQ.Channels.Framing.Writers
{
    internal abstract class FrameWriterBase : IWriteFrames
    {
        private readonly Byte[] preamble = new Byte[Frame.PreambleSize];

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        { }

        public abstract void Write(IList<Frame> frames);

        protected void WriteFramesToStream(IList<Frame> frames, Stream stream)
        {
            //TODO: Issue #11 -- Implement BufferPool (or consider WCF BufferManager?)
            //TODO: Issue #12 -- Consider custom implementation of FrameWriter for sockets.
            var rawMessage = new Byte[frames.Count * Frame.PreambleSize + frames.Sum(frame => frame.Body.Count)];
            var offset = 0;

            foreach (var frame in frames)
            {
                unsafe
                {
                    fixed (Byte* numRef = this.preamble)
                        *((Int32*)numRef) = sizeof(Byte) + frame.Body.Count;

                    this.preamble[Frame.PreambleFlagsOffset] = (Byte)frame.Flags;
                }

                Buffer.BlockCopy(this.preamble, 0, rawMessage, offset, Frame.PreambleSize);
                offset += Frame.PreambleSize;

                Buffer.BlockCopy(frame.Body.Array, frame.Body.Offset, rawMessage, offset, frame.Body.Count);
                offset += frame.Body.Count;
            }

            stream.Write(rawMessage, 0, rawMessage.Length);
        }
    }
}
