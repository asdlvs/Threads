using System;
using System.Collections.Generic;

namespace Threads.Parallel
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class CustomOrderablePartitioner : OrderablePartitioner<CustomItem>
    {
        private long _currentPosition = 0;

        private readonly object lockObj = new object();

        private CustomOrderableEnumerableSource _dataSource;

        public CustomItem[] DataItems { get; set; }

        public CustomOrderablePartitioner(CustomItem[] dataItems)
            : base(true, true, true)
        {
            this.DataItems = dataItems;
            _dataSource = new CustomOrderableEnumerableSource(this);
        }

        public override IList<IEnumerator<KeyValuePair<long, CustomItem>>> GetOrderablePartitions(int partitionCount)
        {
            throw new NotImplementedException();
        }

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<KeyValuePair<long, CustomItem>> GetOrderableDynamicPartitions()
        {
            return _dataSource;
        }

        public Tuple<long, long> GetNextChunk()
        {
            var random = new Random();
            int chunkCount = random.Next(1, 10);

            lock (lockObj)
            {
                if (_currentPosition < this.DataItems.Length)
                {
                    long position = _currentPosition;
                    _currentPosition += chunkCount;
                    return new Tuple<long, long>(position, _currentPosition);
                }
                return new Tuple<long, long>(-1, -1);
            }
        }
    }

    public class CustomOrderableEnumerableSource : IEnumerable<KeyValuePair<long, CustomItem>>
    {
        private readonly CustomOrderablePartitioner _partitioner;

        public CustomOrderableEnumerableSource(CustomOrderablePartitioner partitioner)
        {
            _partitioner = partitioner;
        }

        public IEnumerator<KeyValuePair<long, CustomItem>> GetEnumerator()
        {
            return new CustomOrderableChunkEnumerator(_partitioner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class CustomOrderableChunkEnumerator
    {
        private readonly CustomOrderablePartitioner _partitioner;

        public CustomOrderableChunkEnumerator(CustomOrderablePartitioner partitioner)
        {
            _partitioner = partitioner;
        }

        public IEnumerator<KeyValuePair<long, CustomItem>> GetEnumerator()
        {
            while(true)
            {
                var chunk = this._partitioner.GetNextChunk();
                if (chunk.Item1 == -1 || chunk.Item2 == -1) { break; }

                for (long i = chunk.Item1; i < chunk.Item2; i++)
                {
                    _partitioner.DataItems[i].TaskId = Task.CurrentId ?? -1;
                    yield return new KeyValuePair<long, CustomItem>(i, _partitioner.DataItems[i]);
                }
            }
        }
    }

    public class CustomItem
    {
        public int Order { get; set; }

        public Guid Guid { get; set; }

        public int TaskId { get; set; }
    }
}
