using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.CustomTaskScheduler
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> taskQueue;

        private readonly Thread[] threads;

        public CustomTaskScheduler(int concurrency)
        {
            taskQueue = new BlockingCollection<Task>();
            threads = new Thread[concurrency];

            for (int i = 0; i < threads.Length; i++)
            {
                (threads[i] = new Thread(() =>
                    {
                        foreach (Task t in taskQueue.GetConsumingEnumerable())
                        {
                            this.TryExecuteTask(t);
                        }
                    })).Start();
            }
        }

        /// <summary>
        /// Queues a <see cref="T:System.Threading.Tasks.Task"/> to the scheduler.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task"/> to be queued.</param><exception cref="T:System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
        protected override void QueueTask(Task task)
        {
            if (task.CreationOptions.HasFlag(TaskCreationOptions.LongRunning))
            {
                new Thread(() => this.TryExecuteTask(task)).Start();
            }
            else
            {
                taskQueue.Add(task);
            }
        }

        /// <summary>
        /// Determines whether the provided <see cref="T:System.Threading.Tasks.Task"/> can be executed synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <returns>
        /// A Boolean value indicating whether the task was executed inline.
        /// </returns>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task"/> to be executed.</param><param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.</param><exception cref="T:System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception><exception cref="T:System.InvalidOperationException">The <paramref name="task"/> was already executed.</exception>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (threads.Contains(Thread.CurrentThread))
            {
                return this.TryExecuteTask(task);
            }
            return false;
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return threads.Length;
            }
        }

        /// <summary>
        /// For debugger support only, generates an enumerable of <see cref="T:System.Threading.Tasks.Task"/> instances currently queued to the scheduler waiting to be executed.
        /// </summary>
        /// <returns>
        /// An enumerable that allows a debugger to traverse the tasks currently queued to this scheduler.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">This scheduler is unable to generate a list of queued tasks at this time.</exception>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return taskQueue.ToArray();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
           taskQueue.CompleteAdding();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
