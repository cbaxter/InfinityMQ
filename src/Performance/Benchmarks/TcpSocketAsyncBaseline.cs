using System;
using System.Net;
using System.Net.Sockets;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class TcpSocketAsyncBaseline : ThreadedBenchmark
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
        private Socket serverSocket;
        private Socket clientSocket;
        private Socket channelSocket;

        public TcpSocketAsyncBaseline()
            : base("TCP/IP", "Baseline (Asynchronous)")
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

            SignalClient();

            channelSocket = serverSocket.Accept();
        }

        protected override void SendMessage()
        {
            SendAsyncClientMessage();
        }

        private void SendAsyncClientMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) => { ReceiveAsyncClientMessage(); e.Dispose(); };
            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

            clientSocket.SendAsync(args);
        }

        private void ReceiveAsyncClientMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) => { CaptureClientBytesReceived(e.BytesTransferred); SendAsyncClientMessage(); e.Dispose(); };
            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

            clientSocket.ReceiveAsync(args);
        }

        protected override void ReceiveMessage()
        {
            ReceiveAsyncServerMessage();
        }

        private void ReceiveAsyncServerMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) => ReceiveAsyncServerMessage();
            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

            channelSocket.ReceiveAsync(args);
        }

        private void SendAsyncServerMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) => { CaptureServerBytesReceived(e.BytesTransferred); SendAsyncServerMessage(); };
            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

            channelSocket.SendAsync(args);
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
