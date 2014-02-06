using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    public class ParallelRanges
    {
        public void Example()
        {
            IEnumerable<double> result = ParallelEnumerable
                .Range(0, 10)
                .Where(item => item % 2 == 0)
                .Select(item => Math.Pow(item, 2));

            foreach(double d in result)
            {
                Console.WriteLine("Range element: {0}", d);
            }

            Console.WriteLine("Press enter to continue");
            Console.ReadKey();
            IEnumerable<double> result2 = ParallelEnumerable.Repeat(10, 100).Select(item => Math.Pow(item, 2));

            foreach (double d in result2)
            {
                Console.WriteLine("Repeat element: {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }
    }
}
