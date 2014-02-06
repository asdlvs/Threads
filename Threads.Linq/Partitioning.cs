using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    public class Partitioning
    {
        public void TestStaticPartitioner()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            var partitioner = new StaticPartitioner<int>(sourceData);

            IEnumerable<double> results = partitioner.AsParallel().Select(item => Math.Pow(item, 2));

            foreach(double d in results)
            {
                Console.WriteLine("Enumeration got result {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
