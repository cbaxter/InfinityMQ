using System;
using System.Collections.Generic;
using System.Linq;
using InfinityMQ.Performance.Benchmarks;

namespace InfinityMQ.Performance
{
    public static class Program
    {
        private const Int32 MessageSize = 1024; //TODO: Randomize.
        private const Int32 MessageCount = 100000; //TODO: Configure.

        public static void Main()
        {
            Console.WriteLine("Message Size:\t{0}", MessageSize);
            Console.WriteLine("Message Count:\t{0}", MessageCount);

            var benchmarkGroups = GetBenchmarks().ToList();
            var maxNameLength = benchmarkGroups.SelectMany(group => group).Select(group => group.Name.Length).Max();
            foreach (var benchmarkGroup in benchmarkGroups)
            {
                Console.WriteLine();
                Console.WriteLine(benchmarkGroup.Key);
                Console.WriteLine("--------------------------------------------------");

                foreach (var benchmark in benchmarkGroup)
                {
                    benchmark.MessageCount = MessageCount;
                    benchmark.MessageSize = MessageSize;

                    Console.Write("{0} --> ", benchmark.Name.PadRight(maxNameLength, ' '));

                    benchmark.Run();
                    
                    Console.WriteLine(
                        "Message Latency = {0} [us]; Message Throughput = {1} [msg/s]; Data Throughput = {2} [Mb/s];",
                        benchmark.MessageLatency.ToString("F2").PadLeft(10, ' '),
                        benchmark.MessageThroughput.ToString("F2").PadLeft(10, ' '),
                        benchmark.DataThroughput.ToString("F2").PadLeft(10, ' ')
                    );

                    benchmark.Dispose();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static IEnumerable<IGrouping<String, IBenchmark>> GetBenchmarks()
        {
            return typeof(Program).Assembly
                                  .GetTypes()
                                  .Where(type => typeof(IBenchmark).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                                  .Select(type => (IBenchmark)Activator.CreateInstance(type))
                                  .OrderBy(benchmark => benchmark.Group + '.' + benchmark.Name, StringComparer.OrdinalIgnoreCase)
                                  .GroupBy(benchmark => benchmark.Group);
        }
    }
}
