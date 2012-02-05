using System;
using System.Collections.Generic;
using InfinityMQ.Channels.Framing;

namespace InfinityMQ.Channels.Endpoints
{
    internal interface IEndpoint : IDisposable
    {
        void Bind(Uri uri);
        void WaitForConnection();
        void Connect(Uri uri);
        void Disconnect();

        void Send(params Frame[] frames);
        IEnumerable<Frame> Receive();
    }
}
