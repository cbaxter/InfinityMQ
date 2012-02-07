using System;
using InfinityMQ.Channels.Endpoints;
using InfinityMQ.Serialization;

namespace InfinityMQ.Channels
{
    internal class PublisherChannel : ChannelBase, IPublishMessages
    {
        public PublisherChannel(ICreateEndpoints endpointFactory, ISerializeMessages messageSerializer)
            : base(endpointFactory, messageSerializer)
        { }

        public void Publish(Object message)
        {
            WriteMessage(message);
        }

        public void Publish(Object message, Type type)
        {
            WriteMessage(message, type);
        }

        public void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            WriteSegment(new ArraySegment<Byte>(buffer, offset, count));
        }
    }
}
