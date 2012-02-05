using System;

namespace InfinityMQ
{
    internal struct BufferSize
    {
        private readonly Int32 bufferSize;

        private BufferSize(Int32 bufferSize)
        {
            Verify.True(bufferSize >= 0, "bufferSize", "BufferSize must be greater than or equal to zero.");

            this.bufferSize = bufferSize;
        }

        public static BufferSize FromMegabytes(Int32 value)
        {
            return FromKilobytes(value * 1024);
        }

        public static BufferSize FromKilobytes(Int32 value)
        {
            return FromBytes(value * 1024);
        }

        public static BufferSize FromBytes(Int32 value)
        {
            return new BufferSize(value);
        }

        public static implicit operator Int32(BufferSize value)
        {
            return value.bufferSize;
        }

        public static implicit operator BufferSize(Int32 value)
        {
            return new BufferSize(value);
        }
    }
}
