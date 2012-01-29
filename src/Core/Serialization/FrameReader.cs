using System;
using System.Collections.Generic;
using System.IO;

namespace InfinityMQ.Serialization
{
    internal class FrameReader
    {
        private const Int32 BufferSize = 8192;
        private readonly Queue<Frame> availableFrames = new Queue<Frame>();
        private readonly Byte[] sizeBuffer = new Byte[sizeof(Int32)];
        private readonly Byte[] streamBuffer;
        private readonly Stream baseStream;
        private Int32 workingBufferOffset;
        private Byte[] workingBuffer;
        private Boolean bufferingFrame;

        public FrameReader(Stream stream)
            : this(stream, BufferSize)
        { }

        public FrameReader(Stream stream, Int32 bufferSize)
        {
            Verify.NotNull(stream, "stream");

            this.baseStream = stream;
            this.workingBuffer = this.sizeBuffer;
            this.streamBuffer = new Byte[bufferSize];
        }

        public Frame Read()
        {
            while (availableFrames.Count == 0)
                QueueFramesFromStream();

            return availableFrames.Dequeue();
        }

        //TODO: Issue #13 -- Consider custom implementation of FrameReader for Sockets.
        private void QueueFramesFromStream()
        {
            var availableBytes = this.baseStream.Read(this.streamBuffer, 0, this.streamBuffer.Length);
            var streamOffset = 0;

            while (availableBytes > 0)
            {
                var requiredBytes = this.workingBuffer.Length - this.workingBufferOffset;
                var bytesToConsume = Math.Min(availableBytes, requiredBytes);

                Buffer.BlockCopy(this.streamBuffer, streamOffset, this.workingBuffer, this.workingBufferOffset, bytesToConsume);

                streamOffset += bytesToConsume;
                requiredBytes -= bytesToConsume;
                availableBytes -= bytesToConsume;

                if (requiredBytes > 0)
                {
                    this.workingBufferOffset += bytesToConsume;
                    continue;
                }

                this.workingBufferOffset = 0;
                if (this.bufferingFrame)
                {
                    availableFrames.Enqueue(new Frame(this.workingBuffer));

                    this.workingBuffer = this.sizeBuffer;
                    this.bufferingFrame = false;
                }
                else
                {
                    this.workingBuffer = new Byte[BitConverter.ToInt32(this.workingBuffer, 0)];
                    this.bufferingFrame = true;
                }
            }
        }

    }
}