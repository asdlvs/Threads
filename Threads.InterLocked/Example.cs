using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.InterLocked
{
    using System.Threading.Tasks;

    public class Example
    {
        public class BankAccount
        {
            public int Balance = 0;
        }

        public void LongProcess()
        {
            var account = new BankAccount();
            var tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        int startBalance = account.Balance;
                        int localBalance = startBalance;

                        for (int j = 0; j < 1000; j++)
                        {
                            localBalance++;
                        }

                        int sharedData = System.Threading.Interlocked.CompareExchange(ref account.Balance, localBalance, startBalance);

                        if (sharedData == startBalance)
                        {
                            Console.WriteLine("Shared data updated OK");
                        }
                        else
                        {
                            Console.WriteLine("Shared data changed");
                        }
                    });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.Balance);

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
