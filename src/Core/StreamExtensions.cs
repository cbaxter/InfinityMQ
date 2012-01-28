using System;
using System.IO;

namespace InfinityMQ
{
    internal static class StreamExtensions
    {
        public static void Write(this Stream stream, Byte[] buffer)
        {
            Verify.NotNull(stream, "stream");
            Verify.NotNull(buffer, "buffer");
            
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream stream, ArraySegment<Byte> buffer)
        {
            Verify.NotNull(stream, "stream");

            stream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static Int32 Read(this Stream stream, Byte[] buffer)
        {
            Verify.NotNull(stream, "stream");
            Verify.NotNull(buffer, "buffer");
            
            return stream.Read(buffer, 0, buffer.Length);
        }
    }
}
