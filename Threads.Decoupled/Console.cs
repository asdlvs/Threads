using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Decoupled
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class Console
    {
        private static readonly BlockingCollection<Action> _blockingQueue;

        private static Task _messageWorker;

        static Console()
        {
            _blockingQueue = new BlockingCollection<Action>();
            _messageWorker = Task.Factory.StartNew(() =>
                {
                    foreach (Action action in _blockingQueue.GetConsumingEnumerable())
                    {
                        action.Invoke();
                    }
                }, TaskCreationOptions.LongRunning);
        }

        public static void WriteLine(object value)
        {
            _blockingQueue.Add(() => System.Console.WriteLine(value));
        }

        public static void WriteLine(string format, params object[] values)
        {
            _blockingQueue.Add(() => System.Console.WriteLine(format, values));
        }
    }
}
