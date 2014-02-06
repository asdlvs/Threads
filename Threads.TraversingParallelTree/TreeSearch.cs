using System;

namespace Threads.ParallelTree
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    public class TreeSearch
    {
        public static T SearchTree<T>(Tree<T> tree, Func<T, bool> searchFunction)
        {
            var tokenSource = new CancellationTokenSource();
            TWrapper<T> result = PerformSearch(tree, searchFunction, tokenSource);
            return result == null ? default(T) : result.Value;
        }

        private static TWrapper<T> PerformSearch<T>(Tree<T> tree, Func<T, bool> searchFunction, CancellationTokenSource tokenSource)
        {
            TWrapper<T> result = null;

            if (tree != null)
            {
                if(searchFunction(tree.Data))
                {
                    tokenSource.Cancel();

                    result = new TWrapper<T> { Value = tree.Data };
                }
                else
                {
                    if(tree.LeftNode != null && tree.RightNode != null)
                    {
                        Task<TWrapper<T>> leftTask = Task<TWrapper<T>>.Factory.StartNew(() => PerformSearch(tree.LeftNode, searchFunction, tokenSource), tokenSource.Token);
                        Task<TWrapper<T>> rightTask = Task<TWrapper<T>>.Factory.StartNew(() => PerformSearch(tree.RightNode, searchFunction, tokenSource), tokenSource.Token);

                        try
                        {
                            result = leftTask.Result ?? rightTask.Result ?? null;
                        }
                        catch(AggregateException ex)
                        {
                            ex.Handle(e => true);
                        }
                    }
                }
            }

            return result;
        }

        public static void Main()
        {
            Tree<int> tree = Tree<int>.PopulateTree(new Tree<int>(), new Random(2));
            Console.WriteLine("Введите число для поиска в дереве");
            string input = Console.ReadLine();
            int valueToSearch = int.Parse(input ?? 0.ToString(CultureInfo.InvariantCulture));

            int result = TreeSearch.SearchTree(
                tree,
                item =>
                    {
                        if (item == valueToSearch) { Console.WriteLine("Value: {0}", item); }
                        return item == valueToSearch;
                    });

            Console.WriteLine("Search match ? {0}", result);

            Main();
        }
    }
}
