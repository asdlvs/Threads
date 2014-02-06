using System;
using System.Linq;

namespace Threads.Barrier
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        class BankAccount
        {
            public int Balance { get; set; }
        }

        public void Synchronize()
        {
            var accounts = new BankAccount[5];
            for (int i = 0; i < accounts.Length; i++)
            {
                accounts[i] = new BankAccount();
            }

            int totalBalance = 0;

            var barrier = new System.Threading.Barrier(5, (myBarrier) =>
                {
                    totalBalance = accounts.Sum(account => account.Balance);
                    Console.WriteLine("Total balance: {0}", totalBalance);
                });

            var tasks = new Task[5];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(stateObj =>
                    {
                        var account = (BankAccount)stateObj;
                        var random = new Random();
                        for (int j = 0; j < 1000; j++)
                        {
                            account.Balance += random.Next(1, 100);
                        }

                        Console.WriteLine("Task {0}, phase {1} ended", Task.CurrentId, barrier.CurrentPhaseNumber);
                        barrier.SignalAndWait();

                        account.Balance -= (totalBalance - account.Balance) / 10;

                        Console.WriteLine("Task {0}, phase {1} ended", Task.CurrentId, barrier.CurrentPhaseNumber);
                        barrier.SignalAndWait();
                    }, accounts[i]);
            }

            foreach (var task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void Synchronize2()
        {
            var barrier = new System.Threading.Barrier(10, (instance) => Console.WriteLine("End of phase"));

            var tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                int taskNumber = i;
                tasks[taskNumber] = new Task(() =>
                    {
                        Console.WriteLine("phase 1 of task {0}", taskNumber);
                        barrier.SignalAndWait();

                        Console.WriteLine("phase 2 of task {0}", taskNumber);
                        barrier.SignalAndWait();

                        Console.WriteLine("phase 3 of task {0}", taskNumber);
                        barrier.SignalAndWait();
                    });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("press enter to finish");
            Console.ReadLine();
        }

        public void SynchronizeWithReducingParticipation()
        {
            var barrier = new System.Threading.Barrier(2);

            Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Good task starting phase 0");
                    barrier.SignalAndWait();
                    Console.WriteLine("Good task starting phase 1");
                    barrier.SignalAndWait();
                    Console.WriteLine("Good task completed");
                });

            Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Bad task 1 throwing exception");
                    throw new Exception();
                })
                .ContinueWith(antecedent =>
                    {
                        Console.WriteLine("Reducing the barrier participant count");
                        barrier.RemoveParticipant();
                    }, TaskContinuationOptions.OnlyOnFaulted);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        //TODO: Do not forget to process exception
        public void SynchronizedWithCancellation()
        {
            var barrier = new Barrier(2);
            var tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Good task starting phase 0");
                    barrier.SignalAndWait(tokenSource.Token);
                    Console.WriteLine("Good task starting phase 1");
                    barrier.SignalAndWait(tokenSource.Token);
                }, tokenSource.Token);

            Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Bad task 1 throwing exception");
                    throw new Exception();
                }, tokenSource.Token)
                .ContinueWith((antecedent) =>
                    {
                        Console.WriteLine("Cancelling token");
                        tokenSource.Cancel();
                    }, TaskContinuationOptions.OnlyOnFaulted);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
