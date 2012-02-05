using System;

namespace InfinityMQ.Channels
{
    public interface IConnectEndpoints : IDisposable
    {
        void Connect(Uri uri);
        void Disconnect();
    }
}
