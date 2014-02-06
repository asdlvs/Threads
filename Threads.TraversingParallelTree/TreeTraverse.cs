using System;

namespace Threads.ParallelTree
{
    using System.Threading.Tasks;

    public class TreeTraverse
    {
        public static void TraverseTree<T>(Tree<T> tree, Action<T> action)
        {
            if (tree != null)
            {
                action.Invoke(tree.Data);

                if (tree.LeftNode != null && tree.RightNode != null)
                {
                    Task leftTask = Task.Factory.StartNew(() => TraverseTree(tree.LeftNode, action));
                    Task rightTask = Task.Factory.StartNew(() => TraverseTree(tree.RightNode, action));

                    Task.WaitAll(leftTask, rightTask);
                }
            }
        }

        public static void Main()
        {
            Tree<int> tree = Tree<int>.PopulateTree(new Tree<int>(), new Random());

            TraverseTree(tree, item =>
                                                {
                                                    if (item % 2 == 0)
                                                    {
                                                        Console.WriteLine("Item {0}", item);
                                                    }
                                                });
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
    }
}
