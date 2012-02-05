using System;
using InfinityMQ.Channels.Endpoints;
using InfinityMQ.Serialization;

namespace InfinityMQ.Channels
{
    internal class DuplexChannel : ChannelBase, ISendMessages, IReceiveMessages
    {
        public DuplexChannel(ICreateEndpoints endpointFactory, ISerializeMessages messageSerializer)
            : base(endpointFactory, messageSerializer)
        { }

        public void Send(Object message)
        {
            WriteMessage(message);
        }

        public void Send(Object message, Type type)
        {
            WriteMessage(message, type);
        }

        public void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            WriteSegment(new ArraySegment<Byte>(buffer, offset, count));
        }

        public Object Receive()
        {
            return ReadMessage();
        }

        public Byte[] Read()
        {
            var segment = ReadSegment();
            var result = new Byte[segment.Count];

            Buffer.BlockCopy(segment.Array, segment.Offset, result, 0, segment.Count);

            return result;
        }
    }
}
