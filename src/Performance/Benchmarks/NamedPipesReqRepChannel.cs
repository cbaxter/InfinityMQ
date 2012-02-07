using System;
using System.Diagnostics;
using InfinityMQ.Channels;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesReqRepChannel : ThreadedBenchmark
    {
        private readonly ICreateChannels channelFactory = ChannelFactory.Instance;
        private readonly Uri endpoint = new Uri("ipc://InfinityMQ/Benchmark/NamedPipe");
        private IReceiveMessages serverChannel;
        private ISendMessages clientChannel;

        public NamedPipesReqRepChannel()
            : base("Named Pipes", "REQ/REP Channel")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            this.clientChannel.DisposeIfSet();
            this.serverChannel.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            this.clientChannel = this.channelFactory.CreateSender();
            this.clientChannel.Connect(endpoint);
        }

        protected override void SendMessages()
        {
            var bytesSent = 0;
            var bytesReceived = 0;
            var message = new Byte[MessageSize];
            var expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                this.clientChannel.Write(message, 0, MessageSize);

                bytesSent += MessageSize;
                bytesReceived += this.clientChannel.Read().Length;
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownClient()
        {
            this.clientChannel.Disconnect();
        }

        protected override void SetupServer()
        {
            this.serverChannel = channelFactory.CreateReceiver();
            this.serverChannel.Bind(endpoint);
        }

        protected override void WaitForClient()
        {
            this.serverChannel.WaitForConnection();
        }

        protected override void ReceiveMessages()
        {
            var bytesSent = 0;
            var bytesReceived = 0;
            var expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                var message = this.serverChannel.Read();

                bytesReceived += message.Length;

                this.serverChannel.Write(message, 0, MessageSize);

                bytesSent += MessageSize;
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownServer()
        {
            this.serverChannel.Disconnect();
        }
    }
}
