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
                if (random.Next() % 928530924 == 0)
                    counter++;
                counter = counter + 1;
            }
            // ReSharper disable once FunctionNeverReturns
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

        private static Tuple<int, int> GetThreadPoolThreads()
        {
            int maxWorkers;
            int maxIocp;
            int availWorkers;
            int availIocp;
            ThreadPool.GetMaxThreads(out maxWorkers, out maxIocp);
            ThreadPool.GetAvailableThreads(out availWorkers, out availIocp);
            return new Tuple<int, int>(maxWorkers - availWorkers, maxIocp - availIocp);
        }

        private static void StartCpuCounter()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) =>
            {
                var threads = GetThreadPoolThreads();
                Console.WriteLine(
                    $"{Environment.CurrentManagedThreadId} Cpu usage: {GetCpuUsage()} workers: {threads.Item1}; iocp: {threads.Item2}");
            };
            _timer.Start();
        }
    }
}
