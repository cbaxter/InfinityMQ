using System;
using System.Net;
using System.Net.Sockets;

namespace InfinityMQ.Performance.Benchmarks
{
    //TODO: Implement SocketAsyncEventArgs pool.
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

            SignalServerReady();

            channelSocket = serverSocket.Accept();
        }

        protected override void SendMessage()
        {
            SendAsyncClientMessage();
        }

        private void SendAsyncClientMessage()
        {
            if (ByteCounter.AllClientBytesSent)
                return;

            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      ReceiveAsyncClientMessage();

                                      e.Dispose();
                                  };

            IgnoreDirtySocketShutdown(() => clientSocket.SendAsync(args));
        }

        private void ReceiveAsyncClientMessage()
        {
            if (ByteCounter.AllClientBytesReceived)
                return;

            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      CaptureClientBytesReceived(e.BytesTransferred);

                                      e.Dispose();
                                  };

            IgnoreDirtySocketShutdown(() => clientSocket.ReceiveAsync(args));
        }

        protected override void ReceiveMessage()
        {
            ReceiveAsyncServerMessage();
        }

        private void ReceiveAsyncServerMessage()
        {
            if (ByteCounter.AllServerBytesReceived)
                return;

            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      CaptureServerBytesReceived(e.BytesTransferred);

                                      SendAsyncServerMessage();

                                      e.Dispose();
                                  };

            IgnoreDirtySocketShutdown(() => channelSocket.ReceiveAsync(args));
        }

        private void SendAsyncServerMessage()
        {
            if (ByteCounter.AllServerBytesSent)
                return;

            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      ReceiveAsyncClientMessage();

                                      e.Dispose();
                                  };

            IgnoreDirtySocketShutdown(() => channelSocket.SendAsync(args));
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

        private void IgnoreDirtySocketShutdown(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (ObjectDisposedException)
            { }
        }
    }
}
