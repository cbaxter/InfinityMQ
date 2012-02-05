using System;

namespace InfinityMQ.Channels
{
    public interface IReceiveMessages : IBindEndpoints
    {
        void Send(Object message);
        void Send(Object message, Type type);
        void Write(Byte[] buffer, Int32 offset, Int32 count);

        Object Receive();
        Byte[] Read();
    }
}
