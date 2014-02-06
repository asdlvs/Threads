using System;

namespace Threads.Parallel
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Partitioning
    {
        public void SimplePartitioning()
        {
            var resultData = new double[10000000];

            OrderablePartitioner<Tuple<int, int>> chunkPart = Partitioner.Create(0, resultData.Length, 10000);

            Parallel.ForEach(chunkPart, chunkRang =>
                                            {
                                                for (int i = chunkRang.Item1; i < chunkRang.Item2; i++)
                                                {
                                                    resultData[i] = Math.Pow(i, 2);
                                                }
                                            });

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void OrderablePartitioning()
        {
            IList<string> sourceData = new List<string> { "an", "apple", "a", "day", "keeps", "the", "doctor", "away" };

            var resultData = new string[sourceData.Count];

            OrderablePartitioner<string> op = Partitioner.Create(sourceData);

            Parallel.ForEach(op, (item, loopState, index) =>
                                     {
                                         if (item == "apple") { item = "apricot"; }
                                         resultData[index] = item;

                                         Console.WriteLine("Processing item {0} is {1} in task {2}", index, item, Task.CurrentId);
                                     });

            for(int i = 0; i < resultData.Length; i++)
            {
                Console.WriteLine("Item {0} is {1}", i, resultData[i]);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void OrderablePartition2()
        {
            var items = new int[100];
            var revertItems = new int[items.Length];
            var random = new Random();
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = random.Next(0, 1000);
            }

            OrderablePartitioner<int> orderable = Partitioner.Create(items.OrderBy(i => i));

            Parallel.ForEach(
                orderable,
                (item, state, index) =>
                    {
                        revertItems[revertItems.Length - 1 - index] = item;
                    });

            var zero = revertItems.Select((t, i) => t - items[items.Length - 1 - i]).Sum();
            var defaultArraySum = items.Sum();
            var revertArraySum = revertItems.Sum();

            Console.WriteLine("Default array items sum: {0}", defaultArraySum);
            Console.WriteLine("Revert array items sum: {0}", revertArraySum);
            Console.WriteLine("Should be 0: {0}", zero);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
       
        public void CustomPartitioner()
        {
            var random = new Random();
            var sourceData = new WorkItem[100];

            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = new WorkItem { WorkDuration = random.Next(1, 11) };
            }

            Partitioner<WorkItem> cPartitioner = new ContextPartitioner(sourceData, 10);
            Parallel.ForEach(cPartitioner, item => item.PerformWork());

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void RandomPartitioner()
        {
            Partitioner<RandomItem> rPartitioner = new RandomPartitioner(100, 10);
            Parallel.ForEach(rPartitioner, item => Console.WriteLine("Task: {0}, Amount: {1}, Guid: {2}", item.TaskId, item.Amount, item.Guid));

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void CustomOrderablePartitioner()
        {
            var dataItems = new CustomItem[100];
            var resultItems = new string[dataItems.Length];
            for(int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i] = new CustomItem { Guid = Guid.NewGuid(), Order = i };
            }
            OrderablePartitioner<CustomItem> cOrderablePartitioner = new CustomOrderablePartitioner(dataItems);
            Parallel.ForEach(
                cOrderablePartitioner,
                (item, state, index) => resultItems[index] = string.Format("Index: {0}, TaskId: {1}", index, item.TaskId));

            foreach(string t in resultItems)
            {
                Console.WriteLine(t);
            }
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
