using System;

namespace InfinityMQ.Channels
{
    public interface IBindEndpoints : IDisposable
    {
        void Bind(Uri uri);
        void WaitForConnection();
        void Disconnect();
    }
}
