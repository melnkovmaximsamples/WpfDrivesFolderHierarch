using DevExpress.Mvvm;
using System.Collections.ObjectModel;

namespace TreeView.Models
{
    public class Node : BindableBase
    {
        private readonly ObservableCollection<Node> _nodes;
        public ObservableCollection<Node> Nodes => _nodes;
        public string Name { get; set; }

        public Node()
        {
            _nodes = new ObservableCollection<Node>();
        }
    }
}
