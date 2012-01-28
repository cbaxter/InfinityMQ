using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InfinityMQ.Performance
{
    internal static class PrimitiveObjectPool
    {
        public static PrimitiveObjectPool<T> Create<T>(Int32 itemCount, Func<T> itemFactory)
        {
            var poolItems = new List<T>();
            for (var i = 0; i < itemCount; i++)
                poolItems.Add(itemFactory.Invoke());

            return new PrimitiveObjectPool<T>(poolItems);
        }
    }

    internal sealed class PrimitiveObjectPool<T> : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly BlockingCollection<T> queue;
        private readonly IEnumerable<T> enumerable;

        public PrimitiveObjectPool(IEnumerable<T> items)
        {
            this.queue = new BlockingCollection<T>();
            this.cancellationTokenSource = new CancellationTokenSource();
            
            foreach (var item in items)
                this.queue.Add(item);

            this.enumerable = this.queue.GetConsumingEnumerable(this.cancellationTokenSource.Token);
        }

        public T WaitOne()
        {
            return this.enumerable.FirstOrDefault();
        }

        public void Release(T e)
        {
            this.queue.Add(e);
        }

        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }
    }
}
