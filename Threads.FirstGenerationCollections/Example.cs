using System;

namespace Threads.FirstGenerationCollections
{
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void ProcessSynchronizedQueue()
        {
            //Queue queue = new Queue(); returns unpredictable result
            Queue queue = Queue.Synchronized(new Queue());

            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            queue.Enqueue(j);
                        }
                    });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);
            Console.WriteLine("Items enqueued: {0}", queue.Count);
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        public void ProcessSyncRootQueueMember()
        {
            var queue = new Queue();

            for (int j = 0; j < 1000; j++)
            {
                queue.Enqueue(j);
            }

            int itemCount = 0;

            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        while (queue.Count > 0)
                        {
                            lock (queue.SyncRoot)
                            {
                                if (queue.Count > 0)
                                {
                                    queue.Dequeue();
                                    Interlocked.Increment(ref itemCount);
                                }
                            }
                        }
                    });

                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Items processed: {0}", itemCount);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
