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
            : this(frameReader, frameWriter, false)
        { }

        public TcpEndpoint(IReadFrames frameReader, IWriteFrames frameWriter, Boolean ownsFraming)
            : base(frameReader, frameWriter, ownsFraming)
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (NetworkStream != null)
                Disconnect();
        }

        public override void Bind(Uri uri)
        {
            EnsureDisconnected();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port)); //TODO: Issue #20 - Properly parse URI for Endpoint specification.
            Socket.ReceiveBufferSize = BufferSize.FromKilobytes(64); //TODO: Issue #18 - Option Configuration.
            Socket.SendBufferSize = BufferSize.FromKilobytes(64); //TODO: Issue #18 - Option Configuration.
            Socket.Listen((Int32)SocketOptionName.MaxConnections);
        }

        public override void WaitForConnection()
        {
            NetworkStream = new NetworkStream(Socket.Accept()); //TODO: Issue #19 - Allow for multiple Bind/Connect calls on single channel.
        }

        public override void Connect(Uri uri)
        {
            EnsureDisconnected();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port)); //TODO: Issue #20 - Properly parse URI for Endpoint specification.
            Socket.ReceiveBufferSize = BufferSize.FromKilobytes(64); //TODO: Issue #18 - Option Configuration.
            Socket.SendBufferSize = BufferSize.FromKilobytes(64); //TODO: Issue #18 - Option Configuration.

            NetworkStream = new NetworkStream(Socket);
        }

        public override void Disconnect()
        {
            EnsureConnected();

            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Dispose();
                Socket = null;

                NetworkStream.Dispose();
                NetworkStream = null;
            }
            catch (Exception)
            {
                //TODO: Issue #22 - Proper socket shutdown.
            }
        }
    }
}
