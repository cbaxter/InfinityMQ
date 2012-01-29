using System;
using System.IO;
using InfinityMQ.Serialization;

namespace InfinityMQ.Performance
{
    // Cost of Serialization/Deserialization should not count in benchmark tests. 
    internal class BufferMessageSerializer : ISerializeMessages
    {
        private readonly Int32 messageSize;

        public BufferMessageSerializer(Int32 messageSize)
        {
            this.messageSize = messageSize;
        }
        
        public void Serialize(Object graph, Stream output)
        {
            Verify.NotNull(graph, "graph");
            Verify.NotNull(output, "output");

            var buffer = (Byte[]) graph;

            output.Write(buffer, 0, buffer.Length);
            output.Flush();
        }

        public Object Deserialize(Type type, Stream input)
        {
            Verify.NotNull(type, "type");
            Verify.NotNull(input, "input");

            var buffer = new Byte[messageSize];
            var bytesRemaining = messageSize;

            do
            {
                bytesRemaining -= input.Read(buffer, buffer.Length - bytesRemaining, bytesRemaining);
            } while (bytesRemaining > 0);

            return buffer;
        }
    }
}
