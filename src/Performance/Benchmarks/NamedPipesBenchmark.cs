using System;
using System.IO.Pipes;

namespace InfinityMQ.Performance.Benchmarks
{
    internal abstract class NamedPipesBenchmark : ThreadedBenchmark
    {
        private const String ServerName = ".";
        private const String PipeName = "InfinityMQ.Benchmark.NamedPipe";
        protected NamedPipeClientStream ClientStream { get; private set; }
        protected NamedPipeServerStream ServerStream { get; private set; }

        protected NamedPipesBenchmark(String name)
            : base("Named Pipes", name)
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

        protected abstract override void SendMessages();

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

        protected abstract override void ReceiveMessages();

        protected override void TeardownServer()
        {
            ServerStream.WaitForPipeDrain();
        }
    }
}
