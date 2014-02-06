using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threads.SpeculativeProcessing
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class Cache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _dictionary;

        private readonly BlockingCollection<TKey> _queue;

        private readonly Func<TKey, TKey[]> _speculatorFunction;

        private readonly Func<TKey, TValue> _factoryFunction;

        public Cache(Func<TKey, TValue> factory, Func<TKey, TKey[]> speculator)
        {
            _speculatorFunction = speculator;
            _dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
            _queue = new BlockingCollection<TKey>();

            _factoryFunction = key =>
                                   {
                                       TValue value = factory(key);
                                       _queue.Add(key);
                                       return value;
                                   };

            Task.Factory.StartNew(() =>
                                      {
                                          Parallel.ForEach(
                                              _queue.GetConsumingEnumerable(), 
                                              new ParallelOptions { MaxDegreeOfParallelism = 2},
                                              key =>
                                                  {
                                                      foreach(TKey specKey in _speculatorFunction(key))
                                                      {
                                                          TValue res = _dictionary.GetOrAdd(specKey, new Lazy<TValue>(() => factory(specKey))).Value;
                                                      }
                                                  });
                                      });
        }

        public TValue GetValue(TKey key)
        {
            return _dictionary.GetOrAdd(key, new Lazy<TValue>(() => _factoryFunction(key))).Value;
        }
    }
}
