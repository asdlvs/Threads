using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Pipeline
{
    public class Use
    {
        public static void Main()
        {
            Func<int, double> func1 = input => Math.Pow(input, 2);
            Func<double, double> func2 = input => input / 2;
            Func<double, bool> func3 = input => input % 2 == 0 && input > 100;

            Action<int, bool> callback = (input, output) =>
                                             {
                                                 if(output)
                                                 {
                                                     Console.WriteLine("Found value {0} with result {1}", input, output);
                                                 }
                                             };
            Pipeline<int, bool> pipe = new Pipeline<int, double>(func1)
                .AddFunction(func2)
                .AddFunction(func3);

            pipe.StartProcessing();

            for(int i = 0; i < 1000; i++)
            {
                Console.WriteLine("Added value {0}", i);
                pipe.AddValue(i, callback);
            }

            pipe.StopProcessing();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
