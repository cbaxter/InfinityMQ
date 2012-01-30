using System;
using System.IO;
using System.Linq;

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

        public void Write(params Frame[] frames)
        {
            //TODO: Issue #11 -- Implement BufferPool (or consider WCF BufferManager?)
            //TODO: Issue #12 -- Consider custom implementation of FrameWriter for sockets.
            var rawMessage = new Byte[frames.Length * Frame.PreambleSize + frames.Sum(frame => frame.Body.Count)];
            var offset = 0;

            foreach(var frame in frames)
            {
                unsafe
                {
                    fixed (Byte* numRef = preamble)
                        *((Int32*)numRef) = sizeof(Byte) + frame.Body.Count;

                    preamble[Frame.PreambleFlagsOffset] = (Byte)frame.Flags;
                }

                Buffer.BlockCopy(preamble, 0, rawMessage, offset, Frame.PreambleSize);
                offset += Frame.PreambleSize;

                Buffer.BlockCopy(frame.Body.Array, frame.Body.Offset, rawMessage, offset, frame.Body.Count);
                offset += frame.Body.Count;
            }

            this.baseStream.Write(rawMessage, 0, rawMessage.Length);
        }
    }
}
