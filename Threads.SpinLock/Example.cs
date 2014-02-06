using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.SpinLock
{
    using System.Threading.Tasks;

    public class Example
    {
        public class BankAcoount
        {
            public int Balance { get; set; }
        }

        public void LongProcess()
        {
            var account = new BankAcoount();
            var spinlock = new System.Threading.SpinLock();

            var tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            bool acquireLock = false;
                            try
                            {
                                spinlock.Enter(ref acquireLock);
                                account.Balance = account.Balance + 1;
                            }
                            finally
                            {
                                if (acquireLock) { spinlock.Exit(); }
                            }
                        }
                    });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Expect value {0}, Balance: {1}", 10000, account.Balance);
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
