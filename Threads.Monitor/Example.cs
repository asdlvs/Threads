using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Monitor
{
    using System.Threading.Tasks;

    public class Example
    {
        public List<string> SharedData = new List<string>();

        private readonly object syncRoot = new object();

        public void LongProcess()
        {
            var task1 = new Task(this.FirstMember);
            var task2 = new Task(this.SecondMember);

            task1.Start();
            task2.Start();

            Task.WaitAll(task1, task2);
        }

        public void FirstMember()
        {
            for (int i = 0; i < 100; i++)
            {
                bool wasTaken = false;

                try
                {
                    System.Threading.Monitor.Enter(syncRoot, ref wasTaken);
                    SharedData.Add(string.Format("first - {0}", i));
                    Console.WriteLine("first - {0}", i);
                }
                finally
                {
                    if (wasTaken)
                    {
                        System.Threading.Monitor.Exit(syncRoot);
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        public void SecondMember()
        {
            for (int i = 0; i < 100; i++)
            {
                bool wasTaken = false;

                try
                {
                    System.Threading.Monitor.Enter(syncRoot, ref wasTaken);
                    SharedData.Add(string.Format("second - {0}", i));
                    Console.WriteLine("second - {0}", i);
                }
                finally
                {
                    if (wasTaken)
                    {
                        System.Threading.Monitor.Exit(syncRoot);
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
