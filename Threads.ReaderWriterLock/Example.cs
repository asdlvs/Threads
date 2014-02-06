using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.ReaderWriterLock
{
    using System.Threading;
    using System.Threading.Tasks;

    public class Example
    {
        //Exclusively lock
        public void LongProcess()
        {
            var rwLock = new ReaderWriterLockSlim();
            var tokenSource = new CancellationTokenSource();

            var tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                    {
                        while (true)
                        {
                            rwLock.EnterReadLock();
                            Console.WriteLine("Read lock acquired - count: {0}", rwLock.CurrentReadCount);
                            tokenSource.Token.WaitHandle.WaitOne(1000);
                            rwLock.ExitReadLock();
                            Console.WriteLine("Read lock released - count: {0}", rwLock.CurrentReadCount);
                            tokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }, tokenSource.Token);
                tasks[i].Start();
            }

            Console.WriteLine("Press enter to acquire wrute lock");
            Console.ReadLine();

            Console.WriteLine("Requesting write lock");
            rwLock.EnterWriteLock();
            Console.WriteLine("Write lock acquired");
            Console.WriteLine("Press any key to release write lock");
            Console.ReadKey();
            rwLock.ExitWriteLock();

            tokenSource.Token.WaitHandle.WaitOne(2000);
            tokenSource.Cancel();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception was thrown");
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }

        //Nonexclusively lock
        public void LongProcess2()
        {
            var rwLock = new ReaderWriterLockSlim();
            var tokenSource = new CancellationTokenSource();

            int sharedData = 0;
            var readerTasks = new Task[5];

            for(int i = 0; i < readerTasks.Length; i++)
            {
                readerTasks[i] = new Task(() =>
                    {
                        while (true)
                        {
                            rwLock.EnterReadLock();
                            Console.WriteLine("Read lock acquired - count: {0}", rwLock.CurrentReadCount);
                            Console.WriteLine("Shared data value {0}", sharedData);

                            tokenSource.Token.WaitHandle.WaitOne(1000);
                            rwLock.ExitReadLock();
                            Console.WriteLine("Read lock released - count {0}", rwLock.CurrentReadCount);

                            tokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }, tokenSource.Token);
                readerTasks[i].Start();
            }

            var writerTasks = new Task[2];
            for (int i = 0; i < writerTasks.Length; i++)
            {
                writerTasks[i] = new Task(() =>
                    {
                        while (true)
                        {
                            rwLock.EnterUpgradeableReadLock();
                            if (true)
                            {
                                rwLock.EnterWriteLock();
                                Console.WriteLine("Write lock acquired - waiting readers {0}, writers {1}, upgraders {2}", rwLock.WaitingReadCount, rwLock.WaitingWriteCount, rwLock.WaitingUpgradeCount);
                                sharedData++;
                                tokenSource.Token.WaitHandle.WaitOne(1000);
                                rwLock.ExitWriteLock();
                            }
                            rwLock.ExitUpgradeableReadLock();
                            tokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }, tokenSource.Token);
                 writerTasks[i].Start();
            }

            Console.WriteLine("Press enter to cancel tasks.");
            Console.ReadKey();

            tokenSource.Cancel();

            try
            {
                Task.WaitAll(readerTasks);
            }
            catch (AggregateException ex)
            {
                ex.Handle(e => true);
            }

            Console.WriteLine("Press enter to finish");
            Console.ReadKey();
        }
    }
}
