using System;

namespace InfinityMQ
{
    internal static class BufferExtensions
    {
        public static ArraySegment<Byte> ToSegment(this Byte[] buffer, Int32 offset)
        {
            Verify.NotNull(buffer, "buffer");
            
            return new ArraySegment<Byte>(buffer, offset, buffer.Length - offset);
        }

        public static ArraySegment<Byte> ToSegment(this Byte[] buffer, Int32 offset, Int32 count)
        {
            Verify.NotNull(buffer, "buffer");

            return new ArraySegment<Byte>(buffer, offset, count);
        }
    }
}
