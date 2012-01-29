using System;
using System.Collections.Generic;

namespace InfinityMQ.Serialization
{
    internal class FrameDemultiplexer
    {
        private readonly Queue<Frame> availableFrames = new Queue<Frame>();
        private readonly Byte[] sizeBuffer = new Byte[sizeof(Int32)];
        private Int32 workingBufferOffset;
        private Boolean bufferingFrame;
        private Byte[] workingBuffer;

        public Int32 FrameCount { get { return this.availableFrames.Count; } }

        public FrameDemultiplexer()
        {
            this.workingBuffer = sizeBuffer;
        }

        public Frame NextFrame()
        {
            return this.availableFrames.Dequeue();
        }

        public void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            while (count > 0)
            {
                var requiredBytes = this.workingBuffer.Length - this.workingBufferOffset;
                var bytesToConsume = Math.Min(count, requiredBytes);

                Buffer.BlockCopy(buffer, offset, this.workingBuffer, this.workingBufferOffset, bytesToConsume);

                count -= bytesToConsume;
                offset += bytesToConsume;
                requiredBytes -= bytesToConsume;

                if (requiredBytes > 0)
                {
                    this.workingBufferOffset += bytesToConsume;
                    continue;
                }

                this.workingBufferOffset = 0;
                if (this.bufferingFrame)
                {
                    this.availableFrames.Enqueue(new Frame(this.workingBuffer));
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
