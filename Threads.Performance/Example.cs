using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Performance
{
    using System.Threading.Tasks;

    public class Example
    {
        public void Test()
        {
            object lockObj = new object();

            Task[] tasks = new Task[10];
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew( () =>
                                                      {
                                                          lock(lockObj)
                                                          {
                                                              for(int index = 0; index < 50000000; index++)
                                                              {
                                                                  Math.Pow(index, 2);
                                                              }
                                                          }
                                                      });
            }

            Task.WaitAll(tasks);
        }
    }
}
