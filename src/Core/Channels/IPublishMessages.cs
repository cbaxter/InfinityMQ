using System;

namespace InfinityMQ.Channels
{
    public interface IPublishMessages : IChannelMessages
    {
        void Publish(Object message);
        void Publish(Object message, Type type);
        void Write(Byte[] buffer, Int32 offset, Int32 count);
    }
}
