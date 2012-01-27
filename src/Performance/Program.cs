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
            var maxNameLength = benchmarkGroups.SelectMany(group => group).Select(group => group.Name.Length).Max();
            var messageSize = ReadInteger("Enter Message Size (Bytes):\t");
            var messageCount = ReadInteger("Enter Message Count:\t\t");

            foreach (var benchmarkGroup in benchmarkGroups)
            {
                Console.WriteLine();
                Console.WriteLine(benchmarkGroup.Key);
                Console.WriteLine("--------------------------------------------------");

                foreach (var benchmark in benchmarkGroup)
                {
                    Console.Write("{0} --> ", benchmark.Name.PadRight(maxNameLength, ' '));
                    Console.WriteLine(benchmark.Run(messageCount, messageSize));

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
