using System;

namespace InfinityMQ.Channels
{
    public interface IChannelMessages : IDisposable
    {
        void Bind(Uri uri);
        void WaitForConnection();
        void Connect(Uri uri);
        void Disconnect();
    }
}
