using System;

namespace Threads.Mutex
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public class BankAccount
        {
            public int Balance { get; set; }
        }

        public void LongProcess()
        {
            var account = new BankAccount();
            var mutex = new Mutex();

            var tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            bool lockAcquired = mutex.WaitOne();
                            try
                            {
                                account.Balance++;
                            }
                            finally
                            {
                                if (lockAcquired) { mutex.ReleaseMutex(); }
                            }
                        }
                    });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.Balance);
            Console.WriteLine("finish");
            Console.ReadKey();
        }

        public void LongProcess2()
        {
            var account1 = new BankAccount();
            var account2 = new BankAccount();

            var mutex1 = new Mutex();
            var mutex2 = new Mutex();

            var task1 = new Task(() =>
                {
                    bool lockAcquire1 = mutex1.WaitOne();
                    try
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            account1.Balance++;
                        }
                    }
                    finally
                    {
                        if (lockAcquire1) { mutex1.ReleaseMutex(); }
                    }
                });

            var task2 = new Task(() =>
                   {
                       bool lockAcquire2 = mutex2.WaitOne();
                       try
                       {
                           for (int i = 0; i < 1000; i++)
                           {
                               account2.Balance = account2.Balance + 2;
                           }
                       }
                       finally
                       {
                           if (lockAcquire2) { mutex2.ReleaseMutex(); }
                       }

                   });

            var task3 = new Task(() =>
                {
                    bool lockAcquire3 = WaitHandle.WaitAll(new WaitHandle[] { mutex1, mutex2 });
                    try
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            account1.Balance = account1.Balance + 3;
                            account2.Balance = account2.Balance - 1;
                        }
                    }
                    finally
                    {
                        if (lockAcquire3)
                        {
                            mutex1.ReleaseMutex();
                            mutex2.ReleaseMutex();
                        }
                    }
                });

            task1.Start();
            task2.Start();
            task3.Start();

            Task.WaitAll(task1, task2, task3);
            Console.WriteLine("Account1 balance {0}, Account2 balance {1}", account1.Balance, account2.Balance);

            Console.ReadKey();
        }

        public void LongProcess3()
        {
            const string MutexName = "myMutex";

            Mutex namedMutex;

            try
            {
                namedMutex = Mutex.OpenExisting(MutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
               namedMutex = new Mutex(false, MutexName);
            }

            var task = new Task(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("Waiting to acquire Mutex");
                        namedMutex.WaitOne();
                        Console.WriteLine("Acquire Mutex - press enter to release");
                        Console.ReadKey();
                        namedMutex.ReleaseMutex();
                        Console.WriteLine("Release Mutex");
                    }
                });

            
            task.Start();
            task.Wait();
        }
    }
}
