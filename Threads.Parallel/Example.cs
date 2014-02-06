using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Parallel
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void Calculate()
        {
            var dataItems = new int[100];
            var resultItems = new double[100];

            for (int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i] = i;
            }

            Parallel.For(0, dataItems.Length, (index) =>
                {
                    resultItems[index] = Math.Pow(dataItems[index], 2);
                    Console.WriteLine("Default item: {0} result item: {1}", dataItems[index], resultItems[index]);
                });

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }

        public void Invoke()
        {
            Parallel.Invoke(
                () => Console.WriteLine("Action 1"),
                () => Console.WriteLine("Action 2"),
                () => Console.WriteLine("Action 3"));

            var actions = new Action[3];
            actions[0] = () => Console.WriteLine("Action 4");
            actions[1] = () => Console.WriteLine("Action 5");
            actions[2] = () => Console.WriteLine("Action 6");

            Parallel.Invoke(actions);

            Task parent = Task.Factory.StartNew(() =>
                {
                    foreach (var action in actions)
                    {
                        Task.Factory.StartNew(action, TaskCreationOptions.AttachedToParent);
                    }
                });

            parent.Wait();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void For()
        {
            Parallel.For(0, 10, index => Console.WriteLine("Task ID {0} processing index: {1}", Task.CurrentId, index));
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ForEach()
        {
            var dataList = new List<string>
                {
                    "the", "quick", "brown", "fox", "jumps", "etc"
                };

            Parallel.ForEach(dataList, item => Console.WriteLine("Item {0} has {1} characters", item, item.Length));

            Console.WriteLine("press enter to finish");
            Console.ReadLine();
        }

        static IEnumerable<int> SteppedIterator(int startIndex, int endIndex, int stepSize)
        {
            for (int i = startIndex; i < endIndex; i += stepSize)
            {
                yield return i;
            }
        }
        public void EnumerableInForEach()
        {
            Parallel.ForEach(SteppedIterator(0, 10, 2), (index) => Console.WriteLine("Index value: {0}", index));

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void LoopWithParallelOptions()
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };

            Parallel.For(0, 10, options, index =>
                {
                    Console.WriteLine("For Index {0} started", index);
                    Thread.Sleep(500);
                    Console.WriteLine("For Index {0} finished", index);
                });

            var dataElements = new int[] { 0, 2, 4, 6, 8 };

            Parallel.ForEach(dataElements, options, index =>
                {
                    Console.WriteLine("ForEach Index {0} started", index);
                    Thread.Sleep(500);
                    Console.WriteLine("ForEach Index {0} finished", index);
                });

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        //No further iterations will be started
        public void StoppingParallelLoop()
        {
            var dataItems = new List<string> { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };

            Parallel.ForEach(dataItems, (item, state) =>
                {
                    if (item.Contains("k"))
                    {
                        Console.WriteLine("Hit: {0}", item);
                        state.Stop();
                    }
                    else
                    {
                        Console.WriteLine("Miss: {0}", item);
                    }
                });

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        //If you call Break() in the tenth iteration, no further iterations will be started except those that are required to process the first ninth items.
        public void BreakingParallelLoop()
        {
            ParallelLoopResult result = Parallel.For(0, 100, (i, state) =>
                {
                    double sqr = Math.Pow(i, 2);

                    if (sqr > 100)
                    {
                        Console.WriteLine("Breaking on index {0}", i);
                        state.Break();
                    }
                    else
                    {
                        Console.WriteLine("Square value of {0} is {1}", i, sqr);
                    }
                });

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void GettingLoopResult()
        {
            ParallelLoopResult result = Parallel.For(0, 10, (index, state) =>
                {
                    if (index == 5)
                    {
                        state.Stop();
                    }
                });

            Console.WriteLine("Loop result");
            Console.WriteLine("Is Completed: {0}", result.IsCompleted);
            Console.WriteLine("Break Value: {0}", result.LowestBreakIteration.HasValue);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void CancellingParallelLoops()
        {
            var tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(5000);
                    tokenSource.Cancel();

                    Console.WriteLine("Token cancelled");
                });

            var options = new ParallelOptions { CancellationToken = tokenSource.Token };

            try
            {
                Parallel.For(0, long.MaxValue, options, index =>
                    {
                        double result = Math.Pow(index, 3);

                        Console.WriteLine("Index {0}, result {1}", index, result);

                        Thread.Sleep(100);
                    });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Caught cancellation exception...");
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ForThreadLocalStorage()
        {
            int total = 0;

            Parallel.For(0, 100, () => 0, (index, state, tlsValue) =>
                {
                    if (tlsValue == 0) { Console.WriteLine("New thread {0}", Thread.CurrentThread.ManagedThreadId); }
                    tlsValue += index;
                    return tlsValue;
                }, value => Interlocked.Add(ref total, value));

            Console.WriteLine("Total: {0}", total);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ForEachThreadLocalStorage()
        {
            int matchedWords = 0;

            var lockObj = new object();

            var dataItems = new[] { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };

            Parallel.ForEach(dataItems,
                () => 0,
                (item, state, tlsValue) =>
                {
                    if (item.Contains("a"))
                    {
                        tlsValue++;
                    }
                    return tlsValue;
                },
                tlsValue =>
                {
                    lock (lockObj)
                    {
                        matchedWords += tlsValue;
                    }
                });

            Console.WriteLine("Matches: {0}", matchedWords);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        class Transaction
        {
            public int Amount { get; set; }
        }
        public void Dependencies()
        {
            var random = new Random();

            const int ITEM_PER_MONTH = 100000;

            var sourceData = new Transaction[12 * ITEM_PER_MONTH];

            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = new Transaction { Amount = random.Next(-400, 500) };
            }

            var monthlyBalances = new int[12];

            for (int currentMonth = 0; currentMonth < 12; currentMonth++)
            {
                Parallel.For(
                    currentMonth * ITEM_PER_MONTH,
                    (currentMonth + 1) * ITEM_PER_MONTH,
                    () => 0,
                    (index, state, tlsValue) => tlsValue += sourceData[index].Amount,
                    tlsValue => monthlyBalances[currentMonth] += tlsValue);

                if (currentMonth > 0)
                {
                    monthlyBalances[currentMonth] += monthlyBalances[currentMonth - 1];
                }
            }

            for (int i = 0; i < monthlyBalances.Length; i++)
            {
                Console.WriteLine("Month {0} - Balance: {1}", i, monthlyBalances[i]);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
