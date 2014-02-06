using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.CustomTaskScheduler
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void Try()
        {
            int procCount = System.Environment.ProcessorCount;

            var scheduler = new CustomTaskScheduler(procCount);

            Console.WriteLine("Custom scheduler ID: {0}", scheduler.Id);
            Console.WriteLine("Default scheduler ID: {0}", TaskScheduler.Default.Id);

            var tokenSouce = new CancellationTokenSource();

            var task1 = new Task(() =>
                {
                    Console.WriteLine("Task {0} executed by scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id);

                    Task.Factory.StartNew(() => Console.WriteLine("Task {0} executed by parent scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id));

                    Task.Factory.StartNew(() => Console.WriteLine("Task {0} executed by default scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id), tokenSouce.Token, TaskCreationOptions.None, TaskScheduler.Default);
                });

            task1.Start(scheduler);

            task1.ContinueWith(antecedent =>
                {
                    Task.Factory.StartNew(() => Console.WriteLine("Task {0} executed by default scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id));
                });

            task1.ContinueWith(antecedent =>
            {
                Task.Factory.StartNew(() => Console.WriteLine("Task {0} executed by custom scheduler {1}", Task.CurrentId, TaskScheduler.Current.Id));
            }, scheduler);
        }
    }
}
