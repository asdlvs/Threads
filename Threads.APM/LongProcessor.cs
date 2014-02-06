using System;

namespace Threads.APM
{
    public class LongProcessor
    {
        private CalculateDelegate calculateDelegate;

        public long Calculate(int initialValue)
        {
            var result = 0;
            for (int i = initialValue; i < int.MaxValue; i++)
            {
                result = result + i;
            }
            return result;
        }

        private delegate long CalculateDelegate(int initialValue);

        public IAsyncResult BeginCalculate(int initialValue, AsyncCallback callback, object state)
        {
            if (initialValue < 0) { throw new ArgumentException("initialValue"); }
            this.calculateDelegate = this.Calculate;
            return this.calculateDelegate.BeginInvoke(initialValue, callback, state);
        }

        public long EndCalculate(IAsyncResult asyncResult)
        {
            if (asyncResult == null) { throw new ArgumentNullException("asyncResult"); }
            if (this.calculateDelegate == null) { throw new ArgumentException("No BeginCalcalute method was invoked."); }

            try
            {
                return this.calculateDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.calculateDelegate = null;
            }
        }
    }
}
