using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesReqRepBaseline : ThreadedBenchmark
    {
        private const String ServerName = ".";
        private const String PipeName = "InfinityMQ.Benchmark.NamedPipe";
        private NamedPipeServerStream serverStream;
        private NamedPipeClientStream clientStream;

        public NamedPipesReqRepBaseline()
            : base("Named Pipes", "REQ/REP Baseline")
        { }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            this.serverStream.DisposeIfSet();
            this.clientStream.DisposeIfSet();
        }

        protected override void SetupClient()
        {
            this.clientStream = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut);
            this.clientStream.Connect();
        }

        protected override void SendMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesSent += WriteMessage(this.clientStream);
                bytesReceived += ReadMessage(this.clientStream);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownClient()
        {
            this.clientStream.WaitForPipeDrain();
        }

        protected override void SetupServer()
        {
            this.serverStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
        }

        protected override void WaitForClient()
        {
            this.serverStream.WaitForConnection();
        }

        protected override void ReceiveMessages()
        {
            Int32 bytesSent = 0;
            Int32 bytesReceived = 0;
            Int32 expectedBytes = MessageSize * MessageCount;

            for (var i = 0; i < MessageCount; i++)
            {
                bytesReceived += ReadMessage(this.serverStream);
                bytesSent += WriteMessage(this.serverStream);
            }

            Debug.Assert(bytesSent == expectedBytes);
            Debug.Assert(bytesReceived == expectedBytes);
        }

        protected override void TeardownServer()
        {
            this.serverStream.WaitForPipeDrain();
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
