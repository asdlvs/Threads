using System;
using System.Collections.Generic;
using System.Linq;

namespace Threads.MapReduce
{
    public class ParallelMapReduce
    {
        public static IEnumerable<TOutput> MapReduce<TInput, TIntermediate, TKey, TOutput>(
            IEnumerable<TInput> sourceData,
            Func<TInput, IEnumerable<TIntermediate>> mapFunction,
            Func<TIntermediate, TKey> groupFunction,
            Func<IGrouping<TKey, TIntermediate>, TOutput> reduceFunction)
        {
            return sourceData.AsParallel().SelectMany(mapFunction).GroupBy(groupFunction).Select(reduceFunction);
        }

        public static void Main()
        {
            Func<int, IEnumerable<int>> map = value =>
                                                  {
                                                      IList<int> factors = new List<int>();
                                                      for(int i = 1; i < value; i++)
                                                      {
                                                          if (value % i == 0)
                                                          {
                                                              factors.Add(i);
                                                          }
                                                      }
                                                      return factors;
                                                  };

            Func<int, int> group = value => value;

            Func<IGrouping<int, int>, KeyValuePair<int, int>> reduce = grouping => new KeyValuePair<int, int>(grouping.Key, grouping.Count());

            IEnumerable<int> sourceData = Enumerable.Range(1, 50);

            IEnumerable<KeyValuePair<int, int>> result = MapReduce(sourceData, map, group, reduce);

            foreach(KeyValuePair<int, int> keyValuePair in result)
            {
                Console.WriteLine("{0} is a factor {1} times", keyValuePair.Key, keyValuePair.Value);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
