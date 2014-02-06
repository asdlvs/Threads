using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.AutoResetEvent
{
    using System.Threading.Tasks;
    using System.Threading;

    public class Example
    {
        public void Synchronize()
        {
            var arEvent = new AutoResetEvent(false);
            var cancellationToken = new CancellationTokenSource();

            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                    {
                        while (!cancellationToken.Token.IsCancellationRequested)
                        {
                            arEvent.WaitOne();
                            Console.WriteLine("Task {0} released", Task.CurrentId);
                        }
                        cancellationToken.Token.ThrowIfCancellationRequested();
                    }, cancellationToken.Token);
            }

            var signallingTask = Task.Factory.StartNew(() =>
                {
                    while (!cancellationToken.Token.IsCancellationRequested)
                    {
                        cancellationToken.Token.WaitHandle.WaitOne(500);
                        arEvent.Set();
                        Console.WriteLine("Event set");
                    }
                }, cancellationToken.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            cancellationToken.Cancel();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void Synchronize2()
        {
            var arEvent = new AutoResetEvent(false);

            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Console.WriteLine("Task {0} waits for event", Task.CurrentId);
                            arEvent.WaitOne();
                            Console.WriteLine("Task {0} receive event", Task.CurrentId);
                        }
                    });
            }

            var signallingTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Press enter to relese one event");
                        Console.ReadLine();
                        arEvent.Set();
                    }
                });

            signallingTask.Wait();
        }
    }
}
