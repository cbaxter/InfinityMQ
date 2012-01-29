using System;
using System.Diagnostics;
using System.IO;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesReqRepBaseline : NamedPipesBenchmark
    {
        public NamedPipesReqRepBaseline()
            : base("REQ/REP Baseline")
        { }

        protected override void SendMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesSent += WriteMessage(ClientStream);
                bytesReceived += ReadMessage(ClientStream);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void ReceiveMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesReceived += ReadMessage(ServerStream);
                bytesSent += WriteMessage(ServerStream);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        private Int32 WriteMessage(Stream stream)
        {
            stream.Write(new Byte[MessageSize], 0, MessageSize);

            return MessageSize;
        }

        private Int32 ReadMessage(Stream stream)
        {
            var buffer = new Byte[MessageSize];
            var bytesRemaining = MessageSize;

            do
            {
                bytesRemaining -= stream.Read(buffer, buffer.Length - bytesRemaining, bytesRemaining);
            } while (bytesRemaining > 0);

            return MessageSize;
        }
    }
}
