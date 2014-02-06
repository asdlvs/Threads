using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    using System.Collections;
    using System.Collections.Concurrent;

    public class StaticPartitioner<T> : Partitioner<T>
    {
        private T[] _data;

        public StaticPartitioner(T[] data)
        {
            _data = data;
        }

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return false;
            }
        }

        public override IList<IEnumerator<T>> GetPartitions(int partitionCount)
        {
            IList<IEnumerator<T>> list = new List<IEnumerator<T>>();
            int itemsPerEnum = _data.Length / partitionCount;

            for(int i = 0; i < partitionCount - 1; i++)
            {
                list.Add(this.CreateEnum(i * itemsPerEnum, (i + 1) * itemsPerEnum));
            }
            list.Add(this.CreateEnum((partitionCount - 1) * itemsPerEnum, _data.Length));

            return list;
        }

        private IEnumerator<T> CreateEnum(int startIndex, int endIndex)
        {
            int index = startIndex;
            while(index < endIndex)
            {
                yield return _data[index++];
            }
        }
    }
}
