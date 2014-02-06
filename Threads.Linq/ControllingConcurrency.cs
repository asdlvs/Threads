using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Linq
{
    using System.Threading;
    using System.Threading.Tasks;

    public class ControllingConcurrency
    {
        public void ForcingParallelism()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results =
                sourceData.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(item => item % 2 == 0)
                .Select(item => Math.Pow(item, 2));

            foreach(double d in results)
            {
                Console.WriteLine("Result {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void LimitingParallelism()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            ParallelQuery<double> results = 
                sourceData.AsParallel()
                .WithDegreeOfParallelism(2)
                .Where(item => item % 2 == 0)
                .Select(item => Math.Pow(item, 2));

            foreach(double d in results)
            {
                Console.WriteLine("Result {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void ForcingSequentialExecution()
        {
            var sourceData = new int[10];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results =
                sourceData.AsParallel()
                .WithDegreeOfParallelism(2)
                .Where(item => item % 2 == 0)
                .Select(item => Math.Pow(item, 2))
                .AsSequential()
                .Select(item => item * 2);

            foreach(double d in results)
            {
                Console.WriteLine("Result {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void HandlingExceptions()
        {
            var sourceData = new int[100];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results =
                sourceData.AsParallel()
                .Select(item =>
                            {
                                if (item == 45)
                                {
                                    throw new Exception("45");
                                }
                                if(item == 87)
                                {
                                    throw new Exception("87");
                                }
                                return Math.Pow(item, 2);
                            });

            try
            {
                foreach(double d in results)
                {
                    Console.WriteLine("Result {0}", d);
                }
            }
            catch(AggregateException exception)
            {
                exception.Handle(ex =>
                {
                    Console.WriteLine("Handled exception of type: {0} with message {1}", ex.GetType(), ex.Message);
                    return true;
                });
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void CancellingQueries()
        {
            var tokenSource = new CancellationTokenSource();

            var sourceData = new int[1000000];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results = sourceData.AsParallel()
                .WithCancellation(tokenSource.Token)
                .Select(item => Math.Pow(item, 2));

            Task.Factory.StartNew(() =>
                                      {
                                          Thread.Sleep(5000);
                                          tokenSource.Cancel();
                                          Console.WriteLine("Tokensource cancelled");
                                      });

            try
            {
                foreach(double d in results)
                {
                    Console.WriteLine("Result: {0}", d);
                }
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Caight cancellation exception");
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        //All of the results are generated before any of them are made available for enumeration by the foreach loop.
        public void SettingMergeOptions()
        {
            var sourceData = new int[100];
            for(int i = 0; i < sourceData.Length; i++)
            {
                sourceData[i] = i;
            }

            IEnumerable<double> results =
                sourceData.AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .Select(item =>
                            {
                                double resultItem = Math.Pow(item, 2);
                                Console.WriteLine("Produced result {0}", resultItem);
                                return resultItem;
                            });

            foreach(double d in results)
            {
                Console.WriteLine("Enumeration got result {0}", d);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
