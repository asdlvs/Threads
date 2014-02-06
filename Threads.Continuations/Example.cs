using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Continuations
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        class BankAccount
        {
            public int Balance { get; set; }
        }

        public void SimpleContinuation()
        {
            var token = new CancellationTokenSource().Token;

            var antecendTask = new Task<BankAccount>(() =>
                {
                    var account = new BankAccount();
                    for (int i = 0; i < 1000; i++)
                    {
                        Console.WriteLine("Iteration {0}", i);
                        token.WaitHandle.WaitOne(1);
                        account.Balance++;
                    }
                    return account;
                }, token);

            var continuationTask = antecendTask.ContinueWith<int>(
                (Task<BankAccount> antecend) =>
                {
                    Console.WriteLine("Interim Balance: {0}", antecend.Result.Balance);
                    return antecend.Result.Balance * 2;
                });

            antecendTask.Start();

            //Throw Invalid operation exception.
            continuationTask.Start();

            continuationTask.Wait();

            Console.WriteLine("press enter to continuationTask.Result");
            Console.ReadKey();
            Console.WriteLine("Final balance: {0}", continuationTask.Result);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void OneToManyContinuation()
        {
            var rootTask = new Task<BankAccount>(() =>
                {
                    var account = new BankAccount();
                    for (int i = 0; i < 1000; i++)
                    {
                        account.Balance++;
                    }
                    return account;
                });

            Task<int> continuationTask1 = rootTask.ContinueWith((Task<BankAccount> antecent) =>
                {
                    Console.WriteLine("Interim balance 1: {0}", antecent.Result.Balance);
                    return antecent.Result.Balance * 2;
                });

            Task continuationTask2 = continuationTask1
                .ContinueWith((Task<int> antecent) => Console.WriteLine("Final Balance 1: {0}", antecent.Result));

            rootTask
                .ContinueWith((Task<BankAccount> antecent) =>
                {
                    Console.WriteLine("Interim balance 2: {0}", antecent.Result.Balance);
                    return antecent.Result.Balance / 2;
                })
                .ContinueWith((Task<int> antecent) => Console.WriteLine("Final Balance 2: {0}", antecent.Result));

            rootTask.Start();
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void SelectiveContinuation()
        {
            var firstGen = new Task(() =>
                {
                    Console.WriteLine("Message from first generatino task");
                    throw new Exception("Manual throwed exception");
                });

            firstGen.ContinueWith(antecedent =>
                    {
                        Console.WriteLine("Antecendent task faulted with type: {0} and message {1}", antecedent.Exception.GetType(), antecedent.Exception.InnerException);
                    }, TaskContinuationOptions.OnlyOnFaulted);

            firstGen.ContinueWith(antecendent => Console.WriteLine("Antecendent task NOT faulted"), TaskContinuationOptions.NotOnFaulted);

            firstGen.Start();

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }

        public void ManyToOneContinuation()
        {
            var account = new BankAccount();

            var tasks = new Task<int>[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>((stateObj) =>
                    {
                        int isolateBalance = (int)stateObj;

                        for (int j = 0; j < 1000; j++)
                        {
                            isolateBalance++;
                        }
                        return isolateBalance;
                    }, account.Balance);
            }

            Task continuationTask = Task.Factory.ContinueWhenAll<int>(tasks, antecedents =>
                {
                    foreach (Task<int> task in antecedents)
                    {
                        account.Balance += task.Result;
                    }
                });

            foreach (var task in tasks)
            {
                task.Start();
            }

            continuationTask.Wait();
            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.Balance);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void AnyToOneContinuation()
        {
            var tasks = new Task<int>[10];
            var tokenSource = new CancellationTokenSource();

            var rnd = new Random();

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task<int>(() =>
                    {
                        int sleepInterval;
                        lock (rnd)
                        {
                            sleepInterval = rnd.Next(500, 1000);
                        }

                        tokenSource.Token.WaitHandle.WaitOne(sleepInterval);
                        tokenSource.Token.ThrowIfCancellationRequested();
                        return sleepInterval;
                    }, tokenSource.Token);
            }

            Task continuationTask = Task.Factory.ContinueWhenAny<int>(tasks, antecedent => Console.WriteLine("The first task slept fot {0} milliseconds", antecedent.Result));

            foreach (var task in tasks)
            {
                task.Start();
            }
            continuationTask.Wait();
            tokenSource.Cancel();

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }

        public void CancellingContinuation()
        {
            var tokenSource = new CancellationTokenSource();

            var task = new Task(() =>
                {
                    Console.WriteLine("Antecedent running");
                    tokenSource.Token.WaitHandle.WaitOne();
                    tokenSource.Token.ThrowIfCancellationRequested();
                }, tokenSource.Token);

            //Because of tokenSource
            Task neverScheduled = task.ContinueWith(antecedent => Console.WriteLine("This task will never be scheduled"), tokenSource.Token);

            Task badSelective = task.ContinueWith(
                antecedent => Console.WriteLine("This task will never be scheduled"),
                tokenSource.Token,
                TaskContinuationOptions.OnlyOnCanceled,
                TaskScheduler.Current);

            Task goodSelective = task.ContinueWith(antecedent => Console.WriteLine("Continuation running"), TaskContinuationOptions.OnlyOnCanceled);


            task.Start();

            Console.WriteLine("Press enter to cancel token");
            Console.ReadLine();

            tokenSource.Cancel();

            goodSelective.Wait();
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void PropagateExceptions()
        {
            var gen1 = new Task(() => Console.WriteLine("First generation task"));

            Task gen2 = gen1.ContinueWith(antecedent =>
                {
                    Console.WriteLine("Second genetaion task - throw exception");
                    throw new Exception("Manual exception");
                });

            Task gen3 = gen2.ContinueWith(antecedent =>
                {
                    if (antecedent.Status == TaskStatus.Faulted)
                    {
                        throw antecedent.Exception.InnerException;
                    }
                    Console.WriteLine("Third generation task");
                });

            gen1.Start();

            try
            {
                gen3.Wait();
            }
            catch (AggregateException ex)
            {
                ex.Handle(inner =>
                    {
                        Console.WriteLine("Handled exception of type {0}", inner.GetType());
                        return true;
                    });
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }

        public void PropagateExceptionsAnyToOne()
        {
            var tasks = new Task[10];
            var rnd = new Random();

            for (int i = 0; i < tasks.Length; i++)
            {
                int rndInteger = rnd.Next(500, 1000);
                bool throwException = rndInteger % 2 == 0;

                tasks[i] = new Task(() =>
                    {
                        if (throwException)
                        {
                            throw new Exception("Dead!");
                        }
                    });
            }

            // TaskContinuationOptions.NotOnFaulted doesn not work with ContinueWhenAny method.

            Task.Factory.ContinueWhenAny(tasks, antecedent => Console.WriteLine("One task completed: {0}. It is faulted: {1}", antecedent.Id, antecedent.IsFaulted)/*, TaskContinuationOptions.NotOnFaulted*/);

            Task.Factory.ContinueWhenAll(tasks, antecedents =>
            {
                foreach (var task in antecedents)
                {
                    Console.WriteLine(
                        task.Status == TaskStatus.Faulted
                            ? "Task with exception: {0}"
                            : "Task completed successfully: {0}",
                        task.Id);
                }
            });

            foreach (var task in tasks)
            {
                task.Start();
            }


            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }
    }
}
