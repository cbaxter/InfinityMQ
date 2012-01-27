using System;
using System.Net;
using System.Net.Sockets;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class TcpSocketSyncBaseline : ThreadedBenchmark
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
        private Socket serverSocket;
        private Socket clientSocket;
        private Socket channelSocket;

        public TcpSocketSyncBaseline()
            : base("TCP/IP", "Baseline (Synchronous)")
        { }

        protected override void SetupClient()
        {
            clientSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(EndPoint);
        }

        protected override void SetupServer()
        {
            serverSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(EndPoint);
            serverSocket.Listen(1);

            SignalServerReady();

            channelSocket = serverSocket.Accept();
        }

        protected override void SendMessage()
        {
            clientSocket.Send(new Byte[MessageSize]);
            CaptureClientBytesReceived(clientSocket.Receive(new Byte[MessageSize]));
        }

        protected override void ReceiveMessage()
        {
            CaptureServerBytesReceived(channelSocket.Send(new Byte[MessageSize]));
            channelSocket.Receive(new Byte[MessageSize]);
        }

        protected override void TeardownClient()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
        }

        protected override void TeardownServer()
        {
            channelSocket.Shutdown(SocketShutdown.Both);
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (clientSocket != null)
                clientSocket.Dispose();

            if (serverSocket != null)
                serverSocket.Dispose();

            if (channelSocket != null)
                channelSocket.Dispose();
        }
    }
}
