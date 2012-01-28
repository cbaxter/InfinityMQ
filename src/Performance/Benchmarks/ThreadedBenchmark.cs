using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace InfinityMQ.Performance.Benchmarks
{
    internal abstract class ThreadedBenchmark : IBenchmark
    {
        private readonly ManualResetEvent clientCompletedEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent serverCompletedEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent clientReadyEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent serverReadyEvent = new ManualResetEvent(false);

        public String Name { get; private set; }
        public String Group { get; private set; }
        protected Metrics Metrics { get; private set; }
        protected Int32 MessageSize { get { return Metrics.MessageSize; } }
        protected Int32 MessageCount { get { return Metrics.MessageCount; } }

        protected ThreadedBenchmark(String group, String name)
        {
            Group = group;
            Name = name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposing)
                return;

            this.clientReadyEvent.Dispose();
            this.serverReadyEvent.Dispose();
            this.clientCompletedEvent.Dispose();
            this.serverCompletedEvent.Dispose();
        }

        public Metrics Run(Int32 messageCount, Int32 messageSize)
        {
            Metrics = new Metrics(messageCount, messageSize);

            var clientTask = Task.Factory.StartNew(() =>
                                                    {
                                                        this.serverReadyEvent.WaitOne();

                                                        SetupClient();

                                                        this.clientReadyEvent.Set();

                                                        MeasureLatency();

                                                        this.clientCompletedEvent.Set();
                                                        this.serverCompletedEvent.WaitOne();

                                                        TeardownClient();
                                                    });

            var serverTask = Task.Factory.StartNew(() =>
                                                    {
                                                        SetupServer();

                                                        this.serverReadyEvent.Set();

                                                        WaitForClient();

                                                        this.clientReadyEvent.WaitOne();

                                                        MeasureThroughput();

                                                        this.serverCompletedEvent.Set();
                                                        this.clientCompletedEvent.WaitOne();

                                                        TeardownServer();
                                                    });

            Task.WaitAll(clientTask, serverTask);

            return Metrics;
        }

        protected abstract void SetupClient();

        private void MeasureLatency()
        {
            var stopwatch = Stopwatch.StartNew();

            SendMessages();

            stopwatch.Stop();

            Metrics.CalculateLatency(stopwatch.ElapsedTicks);
        }

        protected abstract void SendMessages();

        protected abstract void TeardownClient();
        
        protected abstract void SetupServer();

        protected abstract void WaitForClient();

        private void MeasureThroughput()
        {
            var stopwatch = Stopwatch.StartNew();

            ReceiveMessages();

            stopwatch.Stop();

            Metrics.CalculateThroughput(stopwatch.ElapsedTicks);
        }

        protected abstract void ReceiveMessages();

        protected abstract void TeardownServer();
    }
}
