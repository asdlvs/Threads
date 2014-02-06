using System;

namespace Threads.SpeculativeProcessing
{
    using System.Linq;
    using System.Threading;

    public class Use
    {
        public static void Main()
        {
            Func<int, double> pFunction = value =>
                                              {
                                                  var random = new Random();
                                                  Thread.Sleep(random.Next(1, 10000));
                                                  return Math.Pow(value, 2);
                                              };

            Func<int, double> pFunction2 = value =>
                                               {
                                                   var random = new Random();
                                                   Thread.Sleep(random.Next(1, 2000));
                                                   return Math.Pow(value, 2);
                                               };

            Action<long, double> callback = (index, result) => Console.WriteLine("Received result of {0} from function {1}", result, index);

            for (int i = 0; i < 10; i++)
            {
                SpeculativeSelection.Compute(i, callback, pFunction, pFunction2);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public static void Main2()
        {
            var cache = new Cache<int, double>(key1 =>
                                                    {
                                                        Console.WriteLine("Created value for key {0}", key1);
                                                        return Math.Pow(key1, 2);
                                                    },
                                               key2 => Enumerable.Range(key2 + 1, 5).ToArray());

            for(int i = 0; i < 100; i++)
            {
                double value = cache.GetValue(i);
                Console.WriteLine("Got result {0} for key {1}", value, i);
                Thread.Sleep(100);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
