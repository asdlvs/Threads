using System;

namespace Threads.SemaphoreSlim
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void Synchronize()
        {
            var semaphore = new SemaphoreSlim(2);
            var tokenSource = new CancellationTokenSource();

            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            semaphore.Wait();
                            Console.WriteLine("Task {0} released", Task.CurrentId);
                        }
                    }, tokenSource.Token);
            }

            Task signallingTask = Task.Factory.StartNew(() =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    tokenSource.Token.WaitHandle.WaitOne(500);
                    semaphore.Release(2);
                    Console.WriteLine("Semaphore released");
                }
                tokenSource.Token.ThrowIfCancellationRequested();
            }, tokenSource.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void Synchronize2()
        {
            var semaphore = new SemaphoreSlim(0);

            int i = 0;
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        semaphore.Wait();
                        Console.WriteLine("One more wait {0}", i++);
                    }
                });

            var signallingTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Enter count of tasks to release");
                        string value = Console.ReadLine();

                        int countToRelease = 0;

                        if (int.TryParse(value, out countToRelease))
                        {
                            Console.WriteLine("Released {0} tasks", countToRelease);
                            semaphore.Release(countToRelease);
                        }
                        else
                        {
                            Console.WriteLine("Cannot convert input string no integer");
                        }

                    }
                });

            signallingTask.Wait();

        }
    }
}
