using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Timer = System.Timers.Timer;

namespace CpuUsageTest
{
    internal static class Program
    {
        private static readonly PerformanceCounter CpuCounter = new PerformanceCounter
        {
            CategoryName = "Processor",
            CounterName = "% Processor Time",
            InstanceName = "_Total"
        };
        private static Timer _timer;

        private static void Main(string[] args)
        {
            StartCpuCounter();

            SpawnThreadsAndJoin(4);
        }

        private static void ThreadFunc()
        {
            var random = new Random();
            var counter = 0;
            while (true)
            {
                if (random.Next() == 982347234)
                    counter++;
                counter = counter + 1;
            }
        }

        private static Thread SpawnThread()
        {
            var thread = new Thread(ThreadFunc);
            thread.Start();
            return thread;
        }

        private static void SpawnThreadsAndJoin(int threadCount)
        {
            var threads = new List<Thread>();
            for (var i = 0; i < threadCount; ++i)
            {
                threads.Add(SpawnThread());
            }
            threads.ForEach(t => t.Join());
        }

        private static int GetCpuUsage() => (int)CpuCounter.NextValue();

        private static void StartCpuCounter()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) => Console.WriteLine($"Cpu usage: {GetCpuUsage()}");
            _timer.Start();
        }
    }
}
