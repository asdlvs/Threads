using System;
using System.Linq;

namespace Threads.MapReduce
{
    public class ParallelMap
    {
        public static TOutput[] Map<TInput, TOutput>(Func<TInput, TOutput> mapFunction, TInput[] input)
        {
            return input.AsParallel().AsOrdered().Select(mapFunction).ToArray();
        }

        public static void Main()
        {
            int[] sourceData = Enumerable.Range(0, 100).ToArray();

            Func<int, double> mapFunction = value => Math.Pow(value, 2);

            double[] resultData = Map(mapFunction, sourceData);

            for(int i = 0; i < sourceData.Length; i++)
            {
                Console.WriteLine("Value {0} mapped to {1}", sourceData[i], resultData[i]);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
