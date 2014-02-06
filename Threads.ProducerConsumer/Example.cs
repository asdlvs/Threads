using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.ProducerConsumer
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        class BankAccount
        {
            public int Balance { get; set; }
        }

        class Deposit
        {
            public int Amount { get; set; }
        }

        public void Synchronize()
        {
            var blockingCollection = new BlockingCollection<Deposit>();

            var producers = new Task[3];
            for (int i = 0; i < producers.Length; i++)
            {
                producers[i] = Task.Factory.StartNew(() =>
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            var deposit = new Deposit { Amount = 100 };
                            blockingCollection.Add(deposit);
                        }
                    });
            }

            Task.Factory.ContinueWhenAll(producers, antecedents =>
                {
                    Console.WriteLine("Signalling production end");
                    blockingCollection.CompleteAdding();
                });

            var account = new BankAccount();

            var consumer = Task.Factory.StartNew(() =>
                {
                    while (!blockingCollection.IsCompleted)
                    {
                        Deposit deposit;

                        if (blockingCollection.TryTake(out deposit))
                        {
                            account.Balance += deposit.Amount;
                        }
                    }
                    Console.WriteLine("Final Balance: {0}", account.Balance);
                });

            consumer.Wait();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void MultipleBlockingSynchronize()
        {
            var bc1 = new BlockingCollection<string>();
            var bc2 = new BlockingCollection<string>();

            var bc3 = new BlockingCollection<string>();

            BlockingCollection<string>[] bc1and2 = { bc1, bc2 };
            BlockingCollection<string>[] bcAll = { bc1, bc2, bc3 };

            var tokenSource = new CancellationTokenSource();

            for (int i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() =>
                    {
                        while (!tokenSource.IsCancellationRequested)
                        {
                            string message = string.Format("Message from task {0}", Task.CurrentId);
                            BlockingCollection<string>.AddToAny(bc1and2, message, tokenSource.Token);

                            tokenSource.Token.WaitHandle.WaitOne(1000);
                        }
                    }, tokenSource.Token);
            }

            for (int i = 0; i < 3; i++)
            {
                Task.Factory.StartNew(() =>
                    {
                        while (!tokenSource.IsCancellationRequested)
                        {
                            string warning = string.Format("Warning from task {0}", Task.CurrentId);
                            bc3.Add(warning, tokenSource.Token);
                            tokenSource.Token.WaitHandle.WaitOne(500);
                        }
                    }, tokenSource.Token);
            }

            for (int i = 0; i < 2; i++)
            {
                Task consumer = Task.Factory.StartNew(() =>
                    {
                        string item;
                        while (!tokenSource.IsCancellationRequested)
                        {
                            int bcid = BlockingCollection<string>.TakeFromAny(bcAll, out item, tokenSource.Token);
                            Console.WriteLine("From collection {0}: {1}", bcid, item);
                        }
                    }, tokenSource.Token);
            }

            Console.WriteLine("Press enter to cancel tasks");
            Console.ReadLine();

            tokenSource.Cancel();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void GetConsumingEnumerableTest()
        {
            var blockingCollection = new BlockingCollection<string>();
            var tasks = new Task[5];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => blockingCollection.Add(Task.CurrentId.ToString()));
            }

            Task.WaitAll(tasks);

            foreach (var str in blockingCollection.GetConsumingEnumerable())
            {
                Console.WriteLine(str);
                blockingCollection.Add("new shit");
                Thread.Sleep(1000);
            }
        }

        public void GetConsumingEnumerableTest2()
        {
            var blockingCollection = new BlockingCollection<string>();
            var tasks = new Task[5];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            blockingCollection.Add(Task.CurrentId.ToString());
                            Thread.Sleep(5000);
                        }
                    });
            }

            foreach (var str in blockingCollection.GetConsumingEnumerable())
            {
                Console.WriteLine(str);
            }
        }

        public void GetConsumingEnumerableTestWithThreads()
        {
            var taskQueue = new BlockingCollection<Task>();
            var threads = new Thread[4];

            var cancellationToken = new CancellationToken();
            for (int i = 0; i < 1000; i++)
            {
                taskQueue.Add(new Task(() => cancellationToken.WaitHandle.WaitOne(500)));
            }

            for (int i = 0; i < threads.Length; i++)
            {
                (threads[i] = new Thread(() =>
                {
                    foreach (Task t in taskQueue.GetConsumingEnumerable())
                    {
                        Console.WriteLine("Thread {0} execute task {1}", Thread.CurrentThread.ManagedThreadId, t.Id);
                        Thread.Sleep(500);
                    }
                })).Start();
            }
        }
    }
}
