using System;
using InfinityMQ.Channels.Endpoints;
using InfinityMQ.Serialization;

namespace InfinityMQ.Channels
{
    internal class SubscriberChannel : ChannelBase, ISubscribeToMessages
    {
        public SubscriberChannel(ICreateEndpoints endpointFactory, ISerializeMessages messageSerializer)
            : base(endpointFactory, messageSerializer)
        { }
        
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
