namespace Threads.Parallel
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ContextPartitioner: Partitioner<WorkItem>
    {
        public WorkItem[] DataItems { get; set; }

        protected int targetSum;
        private long sharedStartIndex = 0;
        private object lockObj = new object();
        private EnumerableSource enumSource;

        public ContextPartitioner(WorkItem[] sourceData, int target)
        {
            this.DataItems = sourceData;
            this.targetSum = target;

            this.enumSource = new EnumerableSource(this);
        }

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<WorkItem> GetDynamicPartitions()
        {
            return this.enumSource;
        }

        public override IList<IEnumerator<WorkItem>> GetPartitions(int partitionCount)
        {
            var partitionList = new List<IEnumerator<WorkItem>>();
            IEnumerable<WorkItem> enumObj = this.GetDynamicPartitions();

            for(int i = 0; i < partitionCount; i++)
            {
                partitionList.Add(enumObj.GetEnumerator());
            }

            return partitionList;
        }

        public Tuple<long, long> GetNextChunk()
        {
            Tuple<long, long> result;

            lock(lockObj)
            {
                if (sharedStartIndex < DataItems.Length)
                {
                    int sum = 0;
                    long endIndex = sharedStartIndex;
                    while (endIndex < DataItems.Length && sum < targetSum)
                    {
                        sum += DataItems[endIndex].WorkDuration;
                        endIndex++;
                    }
                    result = new Tuple<long, long>(sharedStartIndex, endIndex);
                    sharedStartIndex = endIndex;
                }
                else
                {
                    result = new Tuple<long, long>(-1, -1);
                }
            }

            return result;
        }
    }

    internal class EnumerableSource : IEnumerable<WorkItem>
    {
        private ContextPartitioner parentPartitioner;

        public EnumerableSource(ContextPartitioner parent)
        {
            this.parentPartitioner = parent;
        }

        public IEnumerator<WorkItem> GetEnumerator()
        {
            return new ChunkEnumerator(parentPartitioner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<WorkItem>)this).GetEnumerator();
        }
    }

    internal class ChunkEnumerator
    {
        private ContextPartitioner parentPartitioner;

        public ChunkEnumerator(ContextPartitioner parentPartitioner)
        {
            this.parentPartitioner = parentPartitioner;
        }

        public IEnumerator<WorkItem> GetEnumerator()
        {
            while(true)
            {
                Tuple<long, long> chunkIndices = parentPartitioner.GetNextChunk();

                if (chunkIndices.Item1 == -1 && chunkIndices.Item2 == -1)
                {
                    break;
                }
                else
                {
                    for (long i = chunkIndices.Item1; i < chunkIndices.Item2; i++)
                    {
                        yield return parentPartitioner.DataItems[i];
                    }
                }
            }
        }
    }

    public class WorkItem
    {
        public int WorkDuration
        {
            get;
            set;
        }

        public void PerformWork()
        {
            Console.WriteLine("Thread.Sleep({0}), TaskID: {1}", this.WorkDuration, Task.CurrentId);
            Thread.Sleep(WorkDuration);
        }
    }

}