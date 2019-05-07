using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPT;

namespace OPT
{
    class Tree
    {
        public Tree(string token)
        {
            root = new TreeNode(token);
        }
        public TreeNode root;
        public void PrintTree()
        {
            Printer(root, 0);
        }
        private void Printer(TreeNode root, int depth)
        {

            for (int i = 0; i < depth; i++)
            {
                Console.Write("    ");
            }
            Console.WriteLine(root.data);
            foreach (TreeNode child in root.children)
            {
                Printer(child, depth + 1);
            }
        }
    }
}