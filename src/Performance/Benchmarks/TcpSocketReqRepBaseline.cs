using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class TcpSocketReqRepBaseline : ThreadedBenchmark
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
        private Socket serverSocket;
        private Socket clientSocket;
        private Socket channelSocket;

        public TcpSocketReqRepBaseline()
            : base("TCP/IP", "REQ/REP Baseline")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            this.clientSocket.DisposeIfSet();
            this.serverSocket.DisposeIfSet();
            this.channelSocket.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            this.clientSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket.Connect(EndPoint);
        }

        protected override void SendMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesSent += SendMessage(this.clientSocket);
                bytesReceived += ReadMessage(this.clientSocket);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownClient()
        {
            this.clientSocket.Shutdown(SocketShutdown.Both);
        }

        protected override void SetupServer()
        {
            this.serverSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(EndPoint);
            this.serverSocket.Listen(1);
        }

        protected override void WaitForClient()
        {
            this.channelSocket = serverSocket.Accept();
        }

        protected override void ReceiveMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesReceived += ReadMessage(this.channelSocket);
                bytesSent += SendMessage(this.channelSocket);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownServer()
        {
            this.channelSocket.Shutdown(SocketShutdown.Both);
        }

        private Int32 SendMessage(Socket socket)
        {
            socket.Send(new Byte[MessageSize]);

            return MessageSize;
        }

        private Int32 ReadMessage(Socket socket)
        {
            var buffer = new Byte[MessageSize];
            var bytesRemaining = MessageSize;

            do
            {
                bytesRemaining -= socket.Receive(buffer, buffer.Length - bytesRemaining, bytesRemaining, SocketFlags.None);
            } while (bytesRemaining > 0);

            return MessageSize;
        }
    }
}
