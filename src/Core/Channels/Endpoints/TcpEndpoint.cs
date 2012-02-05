using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using InfinityMQ.Channels.Framing;
using InfinityMQ.Channels.Framing.Readers;
using InfinityMQ.Channels.Framing.Writers;

namespace InfinityMQ.Channels.Endpoints
{
    internal class TcpEndpoint : EndpointBase
    {
        private Socket Socket { get; set; }
        private NetworkStream NetworkStream { get; set; }
        protected override Stream Stream { get { return NetworkStream; } }
        protected override Boolean Connected { get { return Socket != null || NetworkStream != null; } }

        public TcpEndpoint(IReadFrames frameReader, IWriteFrames frameWriter)
            : base(frameReader, frameWriter)
        { }

        protected override void Dispose(Boolean disposing)
        {
            if (!disposing)
                return;

            if (NetworkStream != null)
                Disconnect();
        }

        public override void Bind(Uri uri)
        {
            EnsureDisconnected();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port)); //TODO: Properly parse uri.
            Socket.Listen((Int32)SocketOptionName.MaxConnections);
        }

        public override void WaitForConnection()
        {
            NetworkStream = new NetworkStream(Socket.Accept()); //TODO: Accept multiple connections.
        }

        public override void Connect(Uri uri)
        {
            EnsureDisconnected();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port));//TODO: Properly parse uri.

            NetworkStream = new NetworkStream(Socket);
        }

        public override void Disconnect()
        {
            EnsureConnected();

            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Disconnect(false);
                Socket.Dispose();
                Socket = null;

                NetworkStream.Dispose();
                NetworkStream = null;
            }
            catch (Exception)
            {
                //TODO: Proper socket shutdown.
            }
        }
    }
}
