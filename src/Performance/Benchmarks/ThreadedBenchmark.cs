﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace InfinityMQ.Performance.Benchmarks
{
    internal abstract class ThreadedBenchmark : IBenchmark
    {
        private readonly ManualResetEvent clientEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent serverEvent = new ManualResetEvent(false);
        private readonly Stopwatch throughputStopwatch = new Stopwatch();
        private readonly Stopwatch latencyStopwatch = new Stopwatch();
        private Int32 totalClientBytesReceived;
        private Int32 totalServerBytesReceived;

        public String Name { get; private set; }
        public String Group { get; private set; }
        public Int32 MessageSize { get; set; }
        public Int32 MessageCount { get; set; }
        public Decimal DataThroughput { get; private set; }
        public Decimal MessageLatency { get; private set; }
        public Decimal MessageThroughput { get; private set; }

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

            if (this.clientEvent != null)
                this.clientEvent.Dispose();

            if (this.serverEvent != null)
                this.serverEvent.Dispose();
        }

        public void Run()
        {
            var clientTask = Task.Factory.StartNew(() =>
                                                    {
                                                        WaitOnServer();
                                                        SetupClient();
                                                        SignalClientReady();
                                                        SendMessages();
                                                        TeardownClient();
                                                    });

            var serverTask = Task.Factory.StartNew(() =>
                                                    {
                                                        SetupServer();
                                                        SignalServerReady();
                                                        WaitOnClient();
                                                        ReceiveMessages();
                                                        TeardownServer();
                                                    });

            Task.WaitAll(clientTask, serverTask);
        }

        #region Client Methods

        protected abstract void SetupClient();

        protected void SignalClientReady()
        {
            this.clientEvent.Set();
        }

        protected void WaitOnClient()
        {
            this.clientEvent.WaitOne();
        }
        protected virtual void SendMessages()
        {
            this.latencyStopwatch.Start();
            for (var i = 0; i < MessageCount; i++)
                SendMessage();
            WaitOnClient();
            this.latencyStopwatch.Stop();

            MessageLatency = this.latencyStopwatch.ElapsedTicks / (Decimal)MessageCount / 2M * 1000000M / Stopwatch.Frequency;
        }

        protected abstract void SendMessage();

        protected Boolean CaptureClientBytesReceived(Int32 count)
        {
            var totalBytes = Interlocked.Increment(ref this.totalClientBytesReceived);
            if (totalBytes == MessageSize * MessageCount)
            {
                SignalClientReady();
                return false;
            }

            return true;
        }

        protected abstract void TeardownClient();

        #endregion

        #region Server Methods

        protected abstract void SetupServer();

        protected void SignalServerReady()
        {
            this.serverEvent.Set();
        }

        protected void WaitOnServer()
        {
            this.serverEvent.WaitOne();
        }

        protected virtual void ReceiveMessages()
        {
            this.throughputStopwatch.Start();
            for (var i = 0; i < MessageCount; i++)
                ReceiveMessage();
            WaitOnServer();
            this.throughputStopwatch.Stop();

            MessageThroughput = (Decimal)MessageCount * Stopwatch.Frequency / this.throughputStopwatch.ElapsedTicks;
            DataThroughput = MessageThroughput * MessageSize * 8 / 1000000;
        }

        protected abstract void ReceiveMessage();

        protected Boolean CaptureServerBytesReceived(Int32 count)
        {
            var totalBytes = Interlocked.Increment(ref this.totalServerBytesReceived);
            if (totalBytes == MessageSize * MessageCount)
            {
                SignalServerReady();
                return false;
            }

            return true;
        }

        protected abstract void TeardownServer();

        #endregion
    }
}
