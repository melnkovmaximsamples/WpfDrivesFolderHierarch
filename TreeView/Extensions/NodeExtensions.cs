using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeView.Models;

namespace TreeView.Extensions
{
    public static class NodeTreeExtensions
    {
        public static void ClearTree(this ObservableCollection<Node> nodes)
        {
            foreach (var child in nodes)
            {
                if (child.Nodes.Count > 0)
                    ClearTree(child.Nodes);
            }

            nodes.Clear();
        }
    }
}
