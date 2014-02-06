using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.Pipeline
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class Pipeline<TInput, TOutput>
    {

        private BlockingCollection<ValueCallBackWrapper> _valueQueue;

        private readonly Func<TInput, TOutput> _pipelineFunction;

        public Pipeline(Func<TInput, TOutput> pipelineFunction)
        {
            _pipelineFunction = pipelineFunction;
        }

        public Pipeline<TInput, TNewOutput> AddFunction<TNewOutput>(Func<TOutput, TNewOutput> newfunction)
        {
            Func<TInput, TNewOutput> compositeFunction = input => newfunction(_pipelineFunction(input));

            return new Pipeline<TInput, TNewOutput>(compositeFunction);
        }

        public void AddValue(TInput value, Action<TInput, TOutput> callback)
        {
            _valueQueue.Add(new ValueCallBackWrapper { Callback = callback, Value = value });
        }

        public void StartProcessing()
        {
            _valueQueue = new BlockingCollection<ValueCallBackWrapper>();

            Task.Factory.StartNew(() =>
                                      {
                                          Parallel.ForEach(
                                              _valueQueue.GetConsumingEnumerable(),
                                              wrapper => wrapper.Callback(wrapper.Value, _pipelineFunction(wrapper.Value)));
                                      });
        }

        public void StopProcessing()
        {
            _valueQueue.CompleteAdding();
        }

        private class ValueCallBackWrapper
        {
            public TInput Value { get; set; }

            public Action<TInput, TOutput> Callback { get; set; }
        }

    }
}
