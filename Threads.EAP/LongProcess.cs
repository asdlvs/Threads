using System;
using System.Collections.Generic;

namespace Threads.EAP
{
    using System.Collections;
    using System.ComponentModel;

    public class LongProcess
    {
        public event CalculateAsyncCompletedEventHandler CalculateCompleted;

        private delegate void CalculateAsyncWorkerProcess(int initValue, AsyncOperation asyncOperation);

        private Dictionary<object, AsyncOperation> tasks = new Dictionary<object, AsyncOperation>();

        private void CalculateCompleteFunc(object state)
        {
            var e = state as CalcalateAsyncCompletedEventArgs;

            if (this.CalculateCompleted != null)
            {
                this.CalculateCompleted(this, e);
            }
        }

        public long Calculate(int initialValue)
        {
            var result = 0;
            for (int i = initialValue; i < int.MaxValue; i++)
            {
                result = result + i;
            }
            return result;
        }

        public void CalculateAsync(int initialValue, object userState)
        {
            AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userState);

            lock (((ICollection)tasks).SyncRoot)
            {
                tasks.Add(userState, asyncOperation);
            }

            var worker = new CalculateAsyncWorkerProcess(CalculateAsyncWorker);
            worker.BeginInvoke(initialValue, asyncOperation, null, null);
        }

        private void CalculateAsyncWorker(int initValue, AsyncOperation asyncOperation)
        {
            var result = this.Calculate(initValue);

            lock (((ICollection)tasks).SyncRoot)
            {
                tasks.Remove(asyncOperation.UserSuppliedState);
            }

            var e = new CalcalateAsyncCompletedEventArgs(null, false, asyncOperation.UserSuppliedState)
                { Result = result };

            asyncOperation.PostOperationCompleted(this.CalculateCompleteFunc, e);

        }

        public bool IsCompleted(object userState)
        {
            return !this.tasks.ContainsKey(userState);
        }

        public delegate void CalculateAsyncCompletedEventHandler(object sender, CalcalateAsyncCompletedEventArgs e);

        public class CalcalateAsyncCompletedEventArgs : AsyncCompletedEventArgs
        {
            public CalcalateAsyncCompletedEventArgs(Exception error, bool cancelled, object userState)
                : base(error, cancelled, userState)
            {
            }

            public long Result { get; set; }
        }
    }
}
