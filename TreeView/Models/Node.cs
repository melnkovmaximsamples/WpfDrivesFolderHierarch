using DevExpress.Mvvm;
using System.Collections.ObjectModel;

namespace TreeView.Models
{
    public class Node : BindableBase
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public string Name { get; set; }

        public Node()
        {
            Nodes = new ObservableCollection<Node>();
        }
    }
}
