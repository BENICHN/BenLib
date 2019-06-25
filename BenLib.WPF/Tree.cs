using BenLib.Standard;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace BenLib.WPF
{
    public class WpfObservableRangeTree<T> : ObservableTreeBase<T>
    {
        public WpfObservableRangeCollection<T> Nodes => (WpfObservableRangeCollection<T>)ObservableItems;
        protected override IList<T> Items => Nodes;

        public WpfObservableRangeTree(WpfObservableRangeCollection<T> nodes) : base(nodes) { foreach (var treeNode in nodes.OfType<ITreeNode<T>>()) if (treeNode.Children is INotifyTreeChanged observableTree) observableTree.TreeChanged += OnNodesTreeChanged; }

        public WpfObservableRangeTree() : this(new WpfObservableRangeCollection<T>()) { }
        public WpfObservableRangeTree(List<T> items) : this(new WpfObservableRangeCollection<T>(items)) { }
        public WpfObservableRangeTree(IEnumerable<T> items) : this(new WpfObservableRangeCollection<T>(items)) { }
        public WpfObservableRangeTree(params T[] items) : this(new WpfObservableRangeCollection<T>(items)) { }
    }
}
