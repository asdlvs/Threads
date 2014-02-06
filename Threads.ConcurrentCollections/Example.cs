using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.ConcurrentCollections
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void ProcessConcurrentQueue()
        {
            var sharedQueue = new ConcurrentQueue<int>();

            for (int i = 0; i < 1000; i++)
            {
                sharedQueue.Enqueue(i);
            }

            int itemCount = 0;
            var tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        while (sharedQueue.Count > 0)
                        {
                            int queueElement;
                            bool gotElement = sharedQueue.TryDequeue(out queueElement);
                            if (gotElement)
                            {
                                Interlocked.Increment(ref itemCount);
                            }
                        }
                    });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
            Console.WriteLine("Item processed: {0}", itemCount);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ProcessConcurrentStack()
        {
            var sharedStack = new ConcurrentStack<int>();

            for (int i = 0; i < 1000; i++)
            {
                sharedStack.Push(i);
            }

            int itemCount = 0;
            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        while (sharedStack.Count > 0)
                        {
                            int stackElement;
                            bool gotElement = sharedStack.TryPop(out stackElement);
                            if (gotElement)
                            {
                                Interlocked.Increment(ref itemCount);
                            }
                        }
                    });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
            Console.WriteLine("Item processed: {0}", itemCount);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ProcessConcurrentBag()
        {
            var sharedBag = new ConcurrentBag<int>();

            for (int i = 0; i < 1000; i++)
            {
                sharedBag.Add(i);
            }

            int itemCount = 0;

            var tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        while (sharedBag.Count > 0)
                        {
                            int bagElement;
                            bool gotElement = sharedBag.TryTake(out bagElement);
                            if (gotElement)
                            {
                                Interlocked.Increment(ref itemCount);
                            }
                        }
                    });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
            Console.WriteLine("Item processed: {0}", itemCount);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        class BankAccount
        {
            public int Balance { get; set; }
        }

        public void ProcessConcurrentDictionary()
        {
            var account = new BankAccount();
            var sharedDict = new ConcurrentDictionary<object, int>();

            var tasks = new Task<int>[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                sharedDict.TryAdd(i, account.Balance);
                tasks[i] = new Task<int>((keyObj) =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            int currentValue;
                            sharedDict.TryGetValue(keyObj, out currentValue);
                            sharedDict.TryUpdate(keyObj, currentValue + 1, currentValue);
                        }

                        int result;
                        bool gotValue = sharedDict.TryGetValue(keyObj, out result);

                        if(gotValue) { return result; }
                        throw new Exception(String.Format("No data item available for key {0}", keyObj));
                    }, i);

                tasks[i].Start();
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                account.Balance += tasks[i].Result;
            }

            Console.WriteLine("Expected value {0}, Balance: {1}", 10000, account.Balance);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
