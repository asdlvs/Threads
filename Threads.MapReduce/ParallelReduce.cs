using System;
using System.Linq;

namespace Threads.MapReduce
{
    public class ParallelReduce
    {
        public static TValue Reduce<TValue>(TValue[] sourceData, TValue seedValue, Func<TValue, TValue, TValue> reduceFunction)
        {
            return sourceData.AsParallel().Aggregate(seedValue, reduceFunction, reduceFunction, value => value);
        }

        public static void Main()
        {
            int[] sourceData = Enumerable.Range(0, 10).ToArray();

            Func<int, int, int> reducaFunction = (i, i1) => i + i1;

            int result = Reduce(sourceData, 0, reducaFunction);

            Console.WriteLine("Result: {0}", result);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
