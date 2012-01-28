using System;
using System.Net;
using System.Net.Sockets;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class TcpSocketAsyncBaseline : ThreadedBenchmark
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
        private PrimitiveObjectPool<SocketAsyncEventArgs> receiveArgsPool;
        private PrimitiveObjectPool<SocketAsyncEventArgs> sendArgsPool;
        private Counter receivedBytesCount;
        private Counter sentBytesCount;
        private Socket serverSocket;
        private Socket clientSocket;
        private Socket channelSocket;

        public TcpSocketAsyncBaseline()
            : base("TCP/IP", "PUB/SUB Baseline")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            this.clientSocket.DisposeIfSet();
            this.serverSocket.DisposeIfSet();
            this.channelSocket.DisposeIfSet();
            this.sendArgsPool.DisposeIfSet();
            this.receiveArgsPool.DisposeIfSet();
            this.sentBytesCount.DisposeIfSet();
            this.receivedBytesCount.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            this.sendArgsPool = PrimitiveObjectPool.Create(100, CreateSendAsyncEventArgs);
            this.sentBytesCount = new Counter(MessageSize * MessageCount);
            this.clientSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket.Connect(EndPoint);
        }

        private SocketAsyncEventArgs CreateSendAsyncEventArgs()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) =>
                                  {
                                      this.sentBytesCount.Add(e.BytesTransferred);
                                      this.sendArgsPool.Release(e);
                                  };

            return args;
        }

        protected override void SendMessages()
        {
            for (var i = 0; i < MessageCount; i++)
            {
                var args = this.sendArgsPool.WaitOne();

                args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

                this.clientSocket.SendAsync(args);
            }

            this.sentBytesCount.WaitForCount();
        }

        protected override void TeardownClient()
        {
            this.clientSocket.Shutdown(SocketShutdown.Both);
        }

        protected override void SetupServer()
        {
            this.receivedBytesCount = new Counter(MessageSize * MessageCount);
            this.receiveArgsPool = PrimitiveObjectPool.Create(100, CreateReceiveAsyncEventArgs);
            this.serverSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(EndPoint);
            this.serverSocket.Listen(1);
        }

        private SocketAsyncEventArgs CreateReceiveAsyncEventArgs()
        {
            var args = new SocketAsyncEventArgs();

            args.Completed += (sender, e) =>
                                  {
                                      this.receivedBytesCount.Add(e.BytesTransferred);

                                      if (!this.receivedBytesCount.CountReached)
                                          ReceiveMessage();

                                      this.receiveArgsPool.Release(e);
                                  };

            return args;
        }

        protected override void WaitForClient()
        {
            this.channelSocket = this.serverSocket.Accept();
        }

        protected override void ReceiveMessages()
        {
            ReceiveMessage();

            this.receivedBytesCount.WaitForCount();
        }

        private void ReceiveMessage()
        {
            var args = this.receiveArgsPool.WaitOne();

            args.SetBuffer(new Byte[MessageSize], 0, MessageSize);

            IgnoreDirtySocketShutdown(() => this.channelSocket.ReceiveAsync(args));
        }

        protected override void TeardownServer()
        {
            this.channelSocket.Shutdown(SocketShutdown.Both);
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
