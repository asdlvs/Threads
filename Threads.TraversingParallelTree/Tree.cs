using System;

namespace Threads.ParallelTree
{
    public class Tree<T>
    {
        public Tree<T> LeftNode { get; set; }

        public Tree<T> RightNode { get; set; }

        public T Data { get; set; }

        internal static Tree<int> PopulateTree(Tree<int> parentNode, Random rnd, int depth = 0)
        {
            parentNode.Data = rnd.Next(1, 1000);
            if (depth < 10)
            {
                parentNode.LeftNode = new Tree<int>();
                parentNode.RightNode = new Tree<int>();

                PopulateTree(parentNode.LeftNode, rnd, depth + 1);
                PopulateTree(parentNode.RightNode, rnd, depth + 1);
            }
            return parentNode;
        }
    }
}
