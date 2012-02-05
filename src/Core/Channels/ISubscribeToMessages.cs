using System;

namespace InfinityMQ.Channels
{
    public interface ISubscribeToMessages : IBindEndpoints
    {
        //void SubscribeAll();
        //void Subscribe(Type type);
        //void Subscribe(Byte[] prefix);
        //void Subscribe<T>(Func<T, Boolean> predicate);

        //void UnsubscribeAll();
        //void Unsubscribe(Type type);
        //void Unsubscribe(Byte[] prefix);
        //void Unsubscribe<T>(Func<T, Boolean> predicate);

        Object Receive();
        Byte[] Read();
    }
}
