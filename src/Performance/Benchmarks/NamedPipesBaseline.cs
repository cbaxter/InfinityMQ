using System;
using System.IO.Pipes;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class NamedPipesBaseline : ThreadedBenchmark
    {
        private const String ServerName = ".";
        private const String PipeName = "InfinityMQ.Benchmark.NamedPipe";
        private NamedPipeServerStream serverStream;
        private NamedPipeClientStream clientStream;

        public NamedPipesBaseline()
            : base("Named Pipes", "Baseline")
        { }

        protected override void SetupClient()
        {
            clientStream = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut);
            clientStream.Connect();
        }

        protected override void SetupServer()
        {
            serverStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut);

            SignalServerReady();

            serverStream.WaitForConnection();
        }

        protected override void SendMessage()
        {
            clientStream.Write(new Byte[MessageSize], 0, MessageSize);
            
            ByteCounter.CaptureClientBytesReceived(clientStream.Read(new Byte[MessageSize], 0, MessageSize));
            if (ByteCounter.AllClientBytesReceived)
                SignalClientReady();
        }

        protected override void ReceiveMessage()
        {
            ByteCounter.CaptureServerBytesReceived(serverStream.Read(new Byte[MessageSize], 0, MessageSize));

            serverStream.Write(new Byte[MessageSize], 0, MessageSize);

            if (ByteCounter.AllServerBytesReceived)
                SignalServerReady();
        }

        protected override void TeardownClient()
        {
            clientStream.WaitForPipeDrain();
        }

        protected override void TeardownServer()
        {
            serverStream.WaitForPipeDrain();
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (serverStream != null)
                serverStream.Dispose();

            if (clientStream != null)
                clientStream.Dispose();
        }
    }
}
