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

            SignalServerReady();

            channelSocket = serverSocket.Accept();
        }

        protected override void SendMessage()
        {
            SendAsyncClientMessage();
        }

        private void SendAsyncClientMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      ReceiveAsyncClientMessage(); 
                                      e.Dispose();
                                  };

            clientSocket.SendAsync(args);
        }

        private void ReceiveAsyncClientMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      var moreBytesExpected = CaptureClientBytesReceived(e.BytesTransferred);
                                      if (!moreBytesExpected)
                                          return;

                                      SendAsyncClientMessage(); 
                                      e.Dispose();
                                  };


            clientSocket.ReceiveAsync(args);
        }

        protected override void ReceiveMessage()
        {
            ReceiveAsyncServerMessage();
        }

        private void ReceiveAsyncServerMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      ReceiveAsyncServerMessage();
                                      e.Dispose();
                                  };

            channelSocket.ReceiveAsync(args);
        }

        private void SendAsyncServerMessage()
        {
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);
            args.Completed += (sender, e) =>
                                  {
                                      var moreBytesExpected = CaptureClientBytesReceived(e.BytesTransferred);
                                      if (!moreBytesExpected)
                                          return;

                                      SendAsyncServerMessage();
                                      e.Dispose();
                                  };

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
