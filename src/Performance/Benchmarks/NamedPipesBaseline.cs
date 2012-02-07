using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesBaseline : ThreadedBenchmark
    {
        private const String ServerName = ".";
        private const String PipeName = "InfinityMQ.Benchmark.NamedPipe";
        protected NamedPipeClientStream ClientStream { get; private set; }
        protected NamedPipeServerStream ServerStream { get; private set; }

        public NamedPipesBaseline()
            : base("Named Pipes", "Baseline")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            ServerStream.DisposeIfSet();
            ClientStream.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            ClientStream = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut);
            ClientStream.Connect();
        }

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

        protected override void TeardownClient()
        {
            ClientStream.WaitForPipeDrain();
        }

        protected override void SetupServer()
        {
            ServerStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
        }

        protected override void WaitForClient()
        {
            ServerStream.WaitForConnection();
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

        protected override void TeardownServer()
        {
            ServerStream.WaitForPipeDrain();
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
