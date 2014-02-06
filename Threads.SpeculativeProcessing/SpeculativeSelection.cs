using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.SpeculativeProcessing
{
    using System.Threading;
    using System.Threading.Tasks;

    public class SpeculativeSelection
    {
        public static void Compute<TInput, TOutput>(TInput value, Action<long, TOutput> callback, params Func<TInput, TOutput>[] functions)
        {
            int resultCounter = 0;

            Task.Factory.StartNew(() =>
                                      {
                                          Parallel.ForEach(functions, (func, state, index) =>
                                                                          {
                                                                              TOutput localResult = func(value);
                                                                              if(Interlocked.Increment(ref resultCounter) == 1)
                                                                              {
                                                                                  state.Stop();
                                                                                  callback(index, localResult);
                                                                              }
                                                                          });
                                      });
        }
    }
}
