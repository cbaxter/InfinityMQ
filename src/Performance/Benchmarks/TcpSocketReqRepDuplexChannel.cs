using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using InfinityMQ.Messaging;
using InfinityMQ.Serialization;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class TcpSocketDuplexChannel : ThreadedBenchmark
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
        private Socket serverSocket;
        private Socket clientSocket;
        private Socket channelSocket;
        private NetworkStream clientNetworkStream;
        private DuplexChannel clientDuplexChannel;
        private NetworkStream channelNetworkStream;
        private DuplexChannel channelDuplexChannel;

        public TcpSocketDuplexChannel()
            : base("TCP/IP", "REQ/REP Channel")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            this.clientSocket.DisposeIfSet();
            this.serverSocket.DisposeIfSet();
            this.channelSocket.DisposeIfSet();
            this.clientNetworkStream.DisposeIfSet();
            this.channelNetworkStream.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            var serializer = new BufferMessageSerializer(MessageSize);

            this.clientSocket = new Socket(EndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket.Connect(EndPoint);

            this.clientNetworkStream = new NetworkStream(this.clientSocket, true);
            this.clientDuplexChannel = new DuplexChannel(
                                           new MessageReader(new FrameReader(this.clientNetworkStream), serializer),
                                           new MessageWriter(new FrameWriter(this.clientNetworkStream), serializer)
                                       );
        }

        protected override void SendMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesSent += SendMessage(this.clientDuplexChannel);
                bytesReceived += ReceiveMessage(this.clientDuplexChannel);
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
            var serializer = new BufferMessageSerializer(MessageSize);

            this.channelSocket = serverSocket.Accept();
            this.channelNetworkStream = new NetworkStream(this.channelSocket, true);
            this.channelDuplexChannel = new DuplexChannel(
                                            new MessageReader(new FrameReader(this.channelNetworkStream), serializer),
                                            new MessageWriter(new FrameWriter(this.channelNetworkStream), serializer)
                                        );
        }

        protected override void ReceiveMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesReceived += ReceiveMessage(this.channelDuplexChannel);
                bytesSent += SendMessage(this.channelDuplexChannel);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownServer()
        {
            this.channelSocket.Shutdown(SocketShutdown.Both);
        }

        private Int32 ReceiveMessage(DuplexChannel channel)
        {
            var buffer = (Byte[])channel.Receive();

            return buffer.Length;
        }

        private Int32 SendMessage(DuplexChannel channel)
        {
            channel.Send(new Byte[MessageSize]);

            return MessageSize;
        }
    }
}
