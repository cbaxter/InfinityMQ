using System;
using System.Threading;

namespace InfinityMQ.Performance
{
    internal sealed class Counter : IDisposable
    {
        private readonly ManualResetEvent countReached = new ManualResetEvent(false);
        private readonly Int64 countTo;
        private Int64 count;

        public Boolean CountReached { get { return Interlocked.Read(ref this.count) >= this.countTo; } }

        public Counter(Int64 countTo)
        {
            this.countTo = countTo;
        }

        public void Add(Int64 value)
        {
            if (Interlocked.Add(ref this.count, value) >= this.countTo)
                this.countReached.Set();
        }

        public void WaitForCount()
        {
            this.countReached.WaitOne();
        }

        public void Dispose()
        {
            this.countReached.Dispose();
        }
    }
}
