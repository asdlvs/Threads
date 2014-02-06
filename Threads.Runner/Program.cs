using System;
using System.Threading;

namespace Threads.Runner
{
    class Program
    {
        public class Balance
        {
            public int Value { get; set; }
        }
        static void Main(string[] args)
        {
            //EapCalcValue();
            //ApmCalcValue();
            //TapCalcValue();

            //new Monitor.Example().LongProcess();
            //new InterLocked.Example().LongProcess();
            //new SpinLock.Example().LongProcess();
            //new Mutex.Example().LongProcess();
            //new Mutex.Example().LongProcess2();
            //new Mutex.Example().LongProcess3();
            //new Declarative.Example().LongProcess();
            //new ReaderWriterLock.Example().LongProcess();
            //new ReaderWriterLock.Example().LongProcess2();
            //new ConcurrentCollections.Example().ProcessConcurrentQueue();
            //new ConcurrentCollections.Example().ProcessConcurrentStack();
            //new ConcurrentCollections.Example().ProcessConcurrentBag();
            //new ConcurrentCollections.Example().ProcessConcurrentDictionary();
            //new FirstGenerationCollections.Example().ProcessSynchronizedQueue();
            //new FirstGenerationCollections.Example().ProcessSyncRootQueueMember();
            //new Continuations.Example().SimpleContinuation();
            //new Continuations.Example().OneToManyContinuation();
            //new Continuations.Example().SelectiveContinuation();
            //new Continuations.Example().ManyToOneContinuation();
            //new Continuations.Example().AnyToOneContinuation();
            //new Continuations.Example().CancellingContinuation();
            //new Continuations.Example().PropagateExceptions();
            //new Continuations.Example().PropagateExceptionsAnyToOne();
            //new ChildTasks.Example().SimpleChildTask();
            //new ChildTasks.Example().AttachedToParentTask();
            //new Barrier.Example().Synchronize();
            //new Barrier.Example().Synchronize2();
            //new Barrier.Example().SynchronizeWithReducingParticipation();
            //new Barrier.Example().SynchronizedWithCancellation();
            //new CountDownEvent.Example().Synchronize();
            //new ManualResetEventSlim.Example().Synchronize();
            //new ManualResetEventSlim.Example().Synchronize2();
            //new AutoResetEvent.Example().Synchronize();
            //new AutoResetEvent.Example().Synchronize2();
            //new SemaphoreSlim.Example().Synchronize();
            //new SemaphoreSlim.Example().Synchronize2();
            //new ProducerConsumer.Example().Synchronize();
            //new ProducerConsumer.Example().MultipleBlockingSynchronize();
            //new ProducerConsumer.Example().GetConsumingEnumerableTest();
            //new ProducerConsumer.Example().GetConsumingEnumerableTest2();
            //new ProducerConsumer.Example().GetConsumingEnumerableTestWithThreads();
            //new CustomTaskScheduler.Example().Try();
            //new Parallel.Example().Calculate();
            //new Parallel.Example().Invoke();
            //new Parallel.Example().For();
            //new Parallel.Example().ForEach();
            //new Parallel.Example().EnumerableInForEach();
            //new Parallel.Example().LoopWithParallelOptions();
            //new Parallel.Example().StoppingParallelLoop();
            //new Parallel.Example().BreakingParallelLoop();
            //new Parallel.Example().GettingLoopResult();
            //new Parallel.Example().CancellingParallelLoops();
            //new Parallel.Example().ForThreadLocalStorage();
            //new Parallel.Example().ForEachThreadLocalStorage();
            //new Parallel.Example().Dependencies();
            //new Parallel.Partitioning().SimplePartitioning();
            //new Parallel.Partitioning().OrderablePartitioning();
            //new Parallel.Partitioning().OrderablePartition2();
            //new Parallel.Partitioning().CustomPartitioner();
            //new Parallel.Partitioning().RandomPartitioner();
            //new Parallel.Partitioning().CustomOrderablePartitioner();
            //new Linq.Example().Test();
            //new Linq.Example().TestAsParallelMethod();
            //new Linq.Example().TestAsParallelOrderedMethod();
            //new Linq.Example().TestOrderedSubqueries();
            //new Linq.Example().NoResultQuery();
            //new Linq.Example().DeferredQueryExecution();
            //new Linq.Example().ImmediateQueryExecution();
            //new Linq.ControllingConcurrency().ForcingParallelism();
            //new Linq.ControllingConcurrency().LimitingParallelism();
            //new Linq.ControllingConcurrency().ForcingSequentialExecution();
            //new Linq.ControllingConcurrency().HandlingExceptions();
            //new Linq.ControllingConcurrency().CancellingQueries();
            //new Linq.ControllingConcurrency().SettingMergeOptions();
            //new Linq.Partitioning().TestStaticPartitioner();
            //new Linq.Aggregation().Test();
            //new Linq.ParallelRanges().Example();
            //new Performance.Example().Test();
            //QuickSort.ParallelSort<int>.Main();
            //ParallelTree.TreeTraverse.Main();
            //ParallelTree.TreeSearch.Main();
            //Cache.ParallelCache<int, double>.Main();
            //MapReduce.ParallelMap.Main();
            //MapReduce.ParallelReduce.Main();
            //MapReduce.ParallelMapReduce.Main();
            //SpeculativeProcessing.Use.Main();
            //SpeculativeProcessing.Use.Main2();
            //Decoupled.Use.Main();
            Pipeline.Use.Main();
        }

        private static void ApmCalcValue()
        {
            var processor = new APM.LongProcessor();
            var result = processor.BeginCalculate(10, null, null);

            while (!result.IsCompleted)
            {
                Console.WriteLine("Poll");
                Thread.Sleep(1000);
            }
            var calcValue = processor.EndCalculate(result);
            Console.WriteLine(calcValue);
        }

        private static void EapCalcValue()
        {
            var processor = new EAP.LongProcess();
            processor.CalculateCompleted += Processor_CalculateCompleted;
            Guid guid = Guid.NewGuid();
            processor.CalculateAsync(10, guid);

            while (!processor.IsCompleted(guid))
            {
                Console.WriteLine("Poll");
                Thread.Sleep(100);
            }
        }

        public static void TapCalcValue()
        {
            var processor = new TAP.LongProcessor();

            var t = processor.CalculateAsync(10);
            t.Start();
            while (!t.IsCompleted)
            {
                Console.WriteLine("poll");
                Thread.Sleep(1000);
            }
            Console.WriteLine(t.Result);
        }

        static void Processor_CalculateCompleted(object sender, EAP.LongProcess.CalcalateAsyncCompletedEventArgs e)
        {
            Console.WriteLine(e.Result);
        }
    }
}
