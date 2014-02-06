using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    public class Aggregation
    {
        public void Test()
        {
            var sourceData = new int[10000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            double aggregateResult = sourceData.AsParallel().Aggregate(
                () => 0.0, //Define initial value for result
                (subtotal, item) => subtotal += Math.Pow(item, 2), // Process each data value
                (total, subtotal) => total + subtotal, // Process each per-Task subtotal
                total => total / 2 // Process final result
                );

            Console.WriteLine("Total: {0}", aggregateResult);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
