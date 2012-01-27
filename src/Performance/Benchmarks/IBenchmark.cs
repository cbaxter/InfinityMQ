using System;

namespace InfinityMQ.Performance.Benchmarks
{
    internal interface IBenchmark : IDisposable
    {
        String Name { get; }
        String Group { get; }
        Decimal DataThroughput { get; }
        Decimal MessageLatency { get; }
        Decimal MessageThroughput { get; }

        Int32 MessageSize { get; set; }
        Int32 MessageCount { get; set; }

        void Run();
    }
}
