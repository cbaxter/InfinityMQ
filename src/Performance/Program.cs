using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using InfinityMQ.Performance.Benchmarks;
using InfinityMQ.Serialization.Serializers;

namespace InfinityMQ.Performance
{
    public static class Program
    {
        public static void Main()
        {
            var messageSize = 1024;// ReadInteger("Enter Message Size (Bytes):\t");
            var messageCount = 10000; // ReadInteger("Enter Message Count:\t\t");

            Decimal tcpLatency = 0;
            Decimal pipeLatency = 0;

            for (var i = 0; i < 20; i++)
            {
                var benchmarkGroups = GetBenchmarks().ToList();
                var maxNameLength = benchmarkGroups.SelectMany(group => group).Select(group => group.Name.Length).Max();

                foreach (var benchmarkGroup in benchmarkGroups)
                {
                    Console.WriteLine();
                    Console.WriteLine(benchmarkGroup.Key);
                    Console.WriteLine("--------------------------------------------------");

                    foreach (var benchmark in benchmarkGroup)
                    {
                        Console.Write("{0} --> ", benchmark.Name.PadRight(maxNameLength, ' '));

                        var metrics = benchmark.Run(messageCount, messageSize);

                        if (benchmark.GetType() == typeof(NamedPipesDuplexChannel))
                            pipeLatency += metrics.MessageLatency;

                        if (benchmark.GetType() == typeof(TcpSocketDuplexChannel))
                            tcpLatency += metrics.MessageLatency;

                        Console.WriteLine(metrics);

                        benchmark.Dispose();

                        //Thread.Sleep(2000);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Pipe Latency = {0}", pipeLatency / 20);
            Console.WriteLine("TCP Latency = {0}", tcpLatency / 20);


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
