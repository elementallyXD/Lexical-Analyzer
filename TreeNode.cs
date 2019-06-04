using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPT
{
    public class TreeNode
    {
        public string data;
        public int code;
        public List<TreeNode> children;
        public TreeNode(string token)
        {
            data = token;
            children = new List<TreeNode>();
        }

        public TreeNode(string token, int code)
        {
            data = token;
            this.code = code;
            children = new List<TreeNode>();
        }

        public TreeNode AddChild(TreeNode child)
        {
            children.Add(child);
            return child;
        }
    }
}
