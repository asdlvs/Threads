using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.ManualResetEventSlim
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void Synchronize()
        {
            var manualResetEvent = new System.Threading.ManualResetEventSlim();
            var tokenSource = new CancellationTokenSource();

            var waitingTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        manualResetEvent.Wait(tokenSource.Token);
                        Console.WriteLine("Waiting task active");
                        tokenSource.Token.WaitHandle.WaitOne(500);
                    }
                }, tokenSource.Token);

            var signallingTask = Task.Factory.StartNew(() =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        tokenSource.Token.WaitHandle.WaitOne(3000);
                        manualResetEvent.Set();
                        Console.WriteLine("Event set");
                        tokenSource.Token.WaitHandle.WaitOne(3000);
                        manualResetEvent.Reset();
                        Console.WriteLine("Event reset");
                    }

                    tokenSource.Token.ThrowIfCancellationRequested();
                }, tokenSource.Token);

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();
            try
            {
                Task.WaitAll(waitingTask, signallingTask);
            }
            catch (AggregateException)
            {
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void Synchronize2()
        {
            var manualResetEventSlim = new ManualResetEventSlim();

            Task waitingTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Start waiting...");
                        manualResetEventSlim.Wait();
                        Console.WriteLine("End waiting");
                        
                        Console.WriteLine("Start reseting");
                        manualResetEventSlim.Reset();
                        Console.WriteLine("End reseting");
                    }
                });

            Task signalingTask = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Press enter to set primitive");
                        Console.ReadLine();
                        Console.WriteLine("Start signalling...");
                        manualResetEventSlim.Set();
                        Console.WriteLine("End signalling");
                    }
                });

            Task.WaitAll(waitingTask, signalingTask);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
