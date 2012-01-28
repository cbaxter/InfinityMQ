using System;
using System.Diagnostics;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class Metrics
    {
        public Int32 MessageCount { get; private set; }
        public Int32 MessageSize { get; private set; }
        public Decimal DataThroughput { get; private set; }
        public Decimal MessageThroughput { get; private set; }
        public Decimal MessageLatency { get; private set; }

        public Metrics(Int32 messageCount, Int32 messageSize)
        {
            MessageCount = messageCount;
            MessageSize = messageSize;
        }

        public void CalculateLatency(Int64 elapsedTicks)
        {
            MessageLatency = elapsedTicks / (Decimal)MessageCount / 2M * 1000000M / Stopwatch.Frequency;
        }

        public void CalculateThroughput(Int64 elapsedTicks)
        {
            MessageThroughput = (Decimal)MessageCount * Stopwatch.Frequency / elapsedTicks;
            DataThroughput = MessageThroughput * MessageSize * 8M / 1000000M;
        }

        public override string ToString()
        {
            return String.Format("Message Latency = {0} [us]; Message Throughput = {1} [msg/s]; Data Throughput = {2} [Mb/s];",
                       MessageLatency.ToString("F2").PadLeft(10, ' '),
                       MessageThroughput.ToString("F2").PadLeft(10, ' '),
                       DataThroughput.ToString("F2").PadLeft(10, ' ')
                   );
        }
    }
}
