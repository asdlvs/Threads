using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    using System.Threading;

    public class Example
    {
        public void Test()
        {
            var sourceData = new int[100];

            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<int> results = sourceData.AsParallel().Where(item => item % 2 == 0);

            foreach (int item in results)
            {
                Console.WriteLine("Item {0}", item);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void TestAsParallelMethod()
        {
            var sourceData = new int[10];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results1 = sourceData.Select(item => Math.Pow(item, 2));

            foreach (double d in results1)
            {
                Console.WriteLine("Sequential result: {0}", d);
            }

            IEnumerable<double> results2 = sourceData.AsParallel().Select(item => Math.Pow(item, 2));

            foreach (double d in results2)
            {
                Console.WriteLine("Parallel result: {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void TestAsParallelOrderedMethod()
        {
            var sourceData = new int[10];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results3 = sourceData.AsParallel().AsOrdered().Select(item => Math.Pow(item, 2));

            foreach (double d in results3)
            {
                Console.WriteLine("Parallel Ordered result: {0}", d);
            }

            var results4 = sourceData.AsParallel().Select(
                item => new
                {
                    sourceValue = item,
                    resultValue = Math.Pow(item, 2)
                });

            foreach (var item in results4.OrderBy(r => r.sourceValue))
            {
                Console.WriteLine("Result: {0} from item {1}", item.resultValue, item.sourceValue);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void TestOrderedSubqueries()
        {
            var sourceData = new int[10000];
            for (int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            var result = sourceData.AsParallel().AsOrdered().Take(10)
                .AsUnordered()
                .Select(
                item => new
                    {
                        sourceValue = item,
                        resultValue = Math.Pow(item, 2)
                    });
            foreach(var item in result)
            {
                Console.WriteLine("Source {0}, Result {1}", item.sourceValue, item.resultValue);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void NoResultQuery()
        {
            var sourceData = new int[50];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            sourceData.AsParallel()
                .Where(item => item % 2 == 0)
                .ForAll(item => Console.WriteLine("Item {0} Result {1}", item, Math.Pow(item, 2)));

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void DeferredQueryExecution()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            Console.WriteLine("Defining PLINQ");

            IEnumerable<double> results = sourceData.AsParallel().Select(item =>
                                                                             {
                                                                                 Console.WriteLine("Processing item {0}", item);
                                                                                 return Math.Pow(item, 2);
                                                                             });

            Console.WriteLine("Waiting...");
            Thread.Sleep(5000);

            Console.WriteLine("Accessing results");

            double total = 0;
            foreach(double result in results)
            {
                total += result;
            }

            Console.WriteLine("Total {0}", total);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ImmediateQueryExecution()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            Console.WriteLine("Defining PLINQ query");

            IEnumerable<double> results = sourceData.AsParallel().Select(item =>
                                                                             {
                                                                                 Console.WriteLine("Processing item {0}", item);
                                                                                 return Math.Pow(item, 2);
                                                                             }).ToArray();

            Console.WriteLine("Waiting...");
            Thread.Sleep(5000);

            Console.WriteLine("Accessing results");
            double total = 0;
            foreach(double result in results)
            {
                total += result;
            }

            Console.WriteLine("Total {0}", total);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
