using System;
using System.Collections.Generic;

namespace Threads.Parallel
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class RandomPartitioner : Partitioner<RandomItem>
    {
        private readonly int _itemsCount;

        private readonly int _partitionSize;

        private readonly Random _random;

        private int _summary;

        private RandomEnumerableSource _source;

        public RandomPartitioner(int itemsCount, int partitionSize)
        {
            _itemsCount = itemsCount;
            _partitionSize = partitionSize;

            _random = new Random();

            _source = new RandomEnumerableSource(this);
        }

        public override bool SupportsDynamicPartitions
        {
            get { return true; }
        }

        public override IEnumerable<RandomItem> GetDynamicPartitions()
        {
            return _source;
        }

        public override IList<IEnumerator<RandomItem>> GetPartitions(int partitionCount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RandomItem> GetNextChunk()
        {
            if (_summary >= _itemsCount) { return null; }

            var block = new List<RandomItem>(_partitionSize);
            for (int i = 0; i < _partitionSize; i++)
            {
                block.Add(new RandomItem { Amount = _random.Next(), Guid = Guid.NewGuid(), TaskId = Task.CurrentId ?? -1 });
            }

            _summary += _partitionSize;
            return block;
        }
    }

    public class RandomEnumerableSource : IEnumerable<RandomItem>
    {
        private readonly RandomPartitioner _partitioner;

        public RandomEnumerableSource(RandomPartitioner partitioner)
        {
            _partitioner = partitioner;
        }

        public IEnumerator<RandomItem> GetEnumerator()
        {
            return new RandomEnum(_partitioner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class RandomEnum
    {
        private readonly RandomPartitioner _partitioner;

        public RandomEnum(RandomPartitioner partitioner)
        {
            _partitioner = partitioner;
        }

        public IEnumerator<RandomItem> GetEnumerator()
        {
            while(true)
            {
                var chunk = _partitioner.GetNextChunk();

                if (chunk == null) { break; }

                foreach(var randomItem in chunk)
                {
                    yield return randomItem;
                }
            }
        }
    }

    public class RandomItem
    {
        public int Amount { get; set; }

        public Guid Guid { get; set; }

        public int TaskId { get; set; }
    }
}
