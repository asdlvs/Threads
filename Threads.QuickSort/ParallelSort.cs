using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Threads.QuickSort
{
    public class ParallelSort<T>
    {
        public static void ParallelQuickSort(T[] data, IComparer<T> comparer, int maxDepth = 16, int minBlockSize = 0)
        {
            Sort(data, 0, data.Length - 1, comparer, 0, maxDepth, minBlockSize);
        }

        internal static void Sort(T[] data, int startIndex, int endIndex, IComparer<T> comparer, int depth, int maxDepth, int minBlockSize)
        {
            if (startIndex < endIndex)
            {
                if (depth > maxDepth || endIndex - startIndex < minBlockSize)
                {
                    Array.Sort(data, startIndex, endIndex - startIndex + 1, comparer);
                }
                else
                {
                    int pivotIndex = PartitionBlock(data, startIndex, endIndex, comparer);

                    Task leftTask = Task.Factory.StartNew(() => Sort(data, startIndex, pivotIndex - 1, comparer, depth + 1, maxDepth, minBlockSize));
                    Task rightTask = Task.Factory.StartNew(() => Sort(data, pivotIndex + 1, endIndex, comparer, depth + 1, maxDepth, minBlockSize));
                    Task.WaitAll(leftTask, rightTask);
                }
            }
        }

        private static int PartitionBlock(T[] data, int startIndex, int endIndex, IComparer<T> comparer)
        {
            T pivot = data[startIndex];

            SwapValues(data, startIndex, endIndex);

            int storeIndex = startIndex;

            for (int i = startIndex; i < endIndex; i++)
            {
                if(comparer.Compare(data[i], pivot) <= 0)
                {
                    SwapValues(data, i, storeIndex);
                    storeIndex++;
                }
            }
            SwapValues(data, storeIndex, endIndex);
            return storeIndex;
        }

        private static void SwapValues(T[] data, int firstIndex, int secondIndex)
        {
            T holder = data[firstIndex];
            data[firstIndex] = data[secondIndex];
            data[secondIndex] = holder;
        }

        public static void Main()
        {
            //var random = new Random();
            //var sourceData = new int[5000000];
            //for (int i = 0; i < sourceData.Length; i++)
            //{
            //    sourceData[i] = random.Next(1, 100);
            //}

            var sourceData = new []
                {
                    6, 5, 7, 6, 3, 2, 9, 1, 1
                };

            ParallelSort<int>.ParallelQuickSort(sourceData, new IntComparer());
        }
    }

    public class IntComparer: IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
