using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Declarative
{
    using System.Runtime.Remoting.Contexts;
    using System.Threading.Tasks;

    public class Example
    {
        [Synchronization]
        public class BankAccount : ContextBoundObject
        {
            private int balance = 0;

            public void IncrementBalance()
            {
                balance++;
            }

            public int GetBalance()
            {
                return balance;
            }
        }

        public void LongProcess()
        {
            var account = new BankAccount();
            var tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            account.IncrementBalance();
                        }
                    });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.GetBalance());

            Console.WriteLine("Precc enter to finish");
            Console.ReadLine();
        }
    }
}
