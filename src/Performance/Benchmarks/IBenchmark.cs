using System;

namespace InfinityMQ.Performance.Benchmarks
{
    internal interface IBenchmark : IDisposable
    {
        String Name { get; }
        String Group { get; }
        Metrics Metrics { get; }

        Metrics Run(Int32 messageCount, Int32 messageSize);
    }
}
