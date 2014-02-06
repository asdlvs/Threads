using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Decoupled
{
    using System.Threading.Tasks;

    public class Use
    {
        public static void Main()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew((state) =>
                                          {
                                              for(int j = 0; j < 10; j++)
                                              {
                                                  Console.WriteLine("Message from task {0}", Task.CurrentId);
                                              }
                                          }, i);
            }

            System.Console.WriteLine("Press enter to finish");
            System.Console.ReadLine();
        }
    }
}
