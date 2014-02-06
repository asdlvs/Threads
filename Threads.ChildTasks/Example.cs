using System;

namespace Threads.ChildTasks
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void SimpleChildTask()
        {
            var parentTask = new Task(() =>
                {
                    Console.WriteLine("In parent task");

                    var childTask = new Task(() =>
                      {
                          Console.WriteLine("Child task running");
                          Thread.Sleep(1000);
                          Console.WriteLine("Child task finished");
                          throw new Exception();
                      });

                    Console.WriteLine("Starting child task...");
                    childTask.Start();
                });

            parentTask.Start();

            Console.WriteLine("Waiting for parent task.");
            parentTask.Wait();
            Console.WriteLine("Parent task finished");

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        public void AttachedToParentTask()
        {
            var parentTask = new Task(() =>
                {
                    var childTask = new Task(() =>
                        {
                            Console.WriteLine("Child 1 running");
                            Thread.Sleep(1000);
                            Console.WriteLine("Child 1 finished");
                            throw new Exception();
                        }, TaskCreationOptions.AttachedToParent);

                    childTask.ContinueWith(antecedent =>
                        {
                            Console.WriteLine("Continuation running");
                            Thread.Sleep(1000);
                            Console.WriteLine("Continuation finished");
                        }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);

                    Console.WriteLine("Starting child task...");
                    childTask.Start();
                });

            parentTask.Start();

            try
            {
                Console.WriteLine("Waiting for parent task");
                parentTask.Wait();
                Console.WriteLine("Parent task finished");
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Exception: {0}", ex.InnerException.GetType());
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
