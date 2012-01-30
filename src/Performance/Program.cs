using System;
using System.Collections.Generic;
using System.Linq;
using InfinityMQ.Performance.Benchmarks;

namespace InfinityMQ.Performance
{
    public static class Program
    {
        public static void Main()
        {
            var benchmarkGroups = GetBenchmarks().ToList();
            var messageSize = ReadInteger("Enter Message Size (Bytes):\t");
            var messageCount = ReadInteger("Enter Message Count:\t\t");

            Console.WriteLine();
            Console.WriteLine("Benchmarked using {0} meesages of {1} bytes each.", messageCount.ToString("N0"), messageSize.ToString("N0"));
           
            foreach (var benchmarkGroup in benchmarkGroups)
            {
                Console.WriteLine();
                Console.WriteLine(benchmarkGroup.Key);
                Console.WriteLine("--------------------------------------------------");

                foreach (var benchmark in benchmarkGroup)
                {
                    var metrics = benchmark.Run(messageCount, messageSize);

                    Console.WriteLine(
                        "{1}{0}{0}- Message Latency = {2} [us]{0}- Message Throughput = {3} [msg/s]{0}- Data Throughput = {4} [Mb/s]{0}",
                        Environment.NewLine,
                        benchmark.Name,
                        metrics.MessageLatency.ToString("F2"),
                        metrics.MessageThroughput.ToString("F2"),
                        metrics.DataThroughput.ToString("F2")
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

        private static Int32 ReadInteger(String prompt)
        {
            Int32 result;

            do
            {
                Console.Write(prompt);
            } while (!Int32.TryParse(Console.ReadLine(), out result) || result <= 0);

            return result;
        }
    }
}
