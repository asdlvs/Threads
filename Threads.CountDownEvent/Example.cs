using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.CountDownEvent
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        public void Synchronize()
        {
            var cdevent = new System.Threading.CountdownEvent(5);
            var random = new Random();

            var tasks = new Task[6];
            for (int i = 0; i < tasks.Length - 1; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        Thread.Sleep(random.Next(500, 1000));
                        Console.WriteLine("Task {0} signalling event", Task.CurrentId);
                        cdevent.Signal();
                    });
            }

            tasks[5] = new Task(() =>
                {
                    Console.WriteLine("Rendezvous task waiting");
                    cdevent.Wait();
                    //cdevent.AddCount(); throw InvalidOperationException;
                    Console.WriteLine("Event has been set");
                });

            foreach (var task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
