using System;
using System.Diagnostics;
using System.IO.Pipes;
using InfinityMQ.Messaging;
using InfinityMQ.Serialization;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesDuplexChannel : NamedPipesBenchmark
    {
        private DuplexChannel clientDuplexChannel;
        private DuplexChannel serverDuplexChannel;

        public NamedPipesDuplexChannel()
            : base("REQ/REP Channel")
        { }

        protected override void SetupClient()
        {
            var serializer = new BufferMessageSerializer(MessageSize);

            base.SetupClient();
            
            this.clientDuplexChannel = new DuplexChannel(
                                           new MessageReader(new FrameReader(ClientStream), serializer),
                                           new MessageWriter(new FrameWriter(ClientStream), serializer)
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

        protected override void WaitForClient()
        {
            var serializer = new BufferMessageSerializer(MessageSize);

            base.WaitForClient();

            this.serverDuplexChannel = new DuplexChannel(
                                           new MessageReader(new FrameReader(ServerStream), serializer),
                                           new MessageWriter(new FrameWriter(ServerStream), serializer)
                                       );
        }

        protected override void ReceiveMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesReceived += ReceiveMessage(this.serverDuplexChannel);
                bytesSent += SendMessage(this.serverDuplexChannel);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
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
