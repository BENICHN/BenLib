using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using BenLib.Standard;
using static BenLib.Standard.Interval<int>;
using static BenLib.Standard.Ordinal<int>;
using System;
using System.Threading.Tasks;

namespace BenLib.Test
{
    [TestClass]
    public class BenLibTest
    {
        public class Permanent
        {
            public static event EventHandler PermanentEvent;
            public int PermanentProperty { get; set; }
        }

        public class Ephemere
        {
            public event EventHandler EphemereEvent;
            public decimal EphemereProperty { get; set; }
        }

        [TestMethod]
        public void MemoryTest()
        {
            void PermanentMethod(object sender, EventArgs e) { }
            for (int i = 0; i < 4000000; i++) new Ephemere().EphemereEvent += PermanentMethod;
            GC.Collect();
            while (true) { }
        }

        [TestMethod]
        public void NumTest()
        {
            double t1 = Num.Solve(x => Num.GetBezierPoint(x, (0, 0), (0.8, 0.5), (0.1, 1), (1, 1)).x - 0.7, 0, 1, 0.001, 0.7);
            var t2 = Num.GetBezierPointFromX(0.7, 0.001, (0, 0), (0.8, 0.5), (0.1, 1), (1, 1));
        }
        [TestMethod]
        public void RangeTest()
        {
            for (int i = 0; i < 50000; i++)
            {
                var rnz = PositiveRealsNoZero | NegativeRealsNoZero;
                bool isr = rnz.IsReals;
                bool isrn = rnz.IsRealsNoZero;
                var issss = new[] { NegativeInfinity, new Ordinal<int>(0, 1) };
                issss.OrderBy(i => i).ToArray();
                var zz = CO(10, 96); //[10 ; 96[
                var sss = new[] { CO(5, 9), OC(5, 9), Reals, OO(6, 8) }; // { [5 ; 9[ , ]5 ; 9] , ℝ , ]6 ; 8[ }
                string str = sss[1].ToString(); //]5 ; 9]
                var sssso = sss.OrderBy(r => r.Start).ToArray(); // { ℝ, [5 ; 9[ , ]5 ; 9] , ]6 ; 8[ }
                var re = OC(0, PositiveInfinity) | CO(NegativeInfinity, 0);
                string rs = re.ToString();
                bool r = 0 <= re && re >= 0;
                var na = CC(5, 9); //[5 ; 9]
                var nb = CO(0, 5); //[0 ; 5[
                var nab = na & nb; //∅
                var z = CC(0, 3) | CC(0, 1); //[0 ; 3]
                var a = CO(6, 6 + 2) | CO(9, 9 + 1); //[6 ; 8[ ∪ [9 ; 10[
                var b = a & CC(0, 4); //∅
                bool c = b > a; //false
                var d = CO(NegativeInfinity, 3) | OC(-5, 6); //]-∞ ; 6]
                d |= CC(9, PositiveInfinity); //]-∞ ; 6[ ∪ [9 ; +∞[
                d &= CC(-20, PositiveInfinity); //[-20 ; 6] ∪ [9 ; +∞[
                bool e = CC(0, 4) > CC(0, 4); //false
                bool e2 = CC(0, 4) >= CC(0, 4); //true
                var f = !(CO(0, 4) | CO(15, 127)); //]-∞ ; 0[ ∪ [4 ; 15[ ∪ [127 ; +∞[
                var g = (CO(-5, 9) | CO(12, PositiveInfinity)) / (CO(NegativeInfinity, 8) | CO(20, 50)); //[8 ; 9[ ∪ [12 ; 20[ ∪ [50 ; +∞[
            }
        }

        [TestMethod]
        public void TreeTest()
        {
            var arrTree = new ArrayTree<Node>
            (
                new Leaf(5), //0
                new Leaf(9), //1
                new TreeNode(new ArrayTree<Node> //2
                (
                    new Leaf(7), //2 → 0
                    new TreeNode(new ArrayTree<Node> //2 → 1
                    (
                        new Leaf(8), //2 → 1 → 0
                        new Leaf(10), //2 → 1 → 1
                        new Leaf(11) //2 → 1 → 2
                    )),
                    new Leaf(80) //2 → 2
                )) { Indice = 0.8 },
                new TreeNode(new ArrayTree<Node> //3
                (
                    new Leaf(57), //3 → 0
                    new TreeNode(new ArrayTree<Node> //3 → 1
                    (
                        new Leaf(58), //3 → 1 → 0
                        new Leaf(510), //3 → 1 → 1
                        new TreeNode(new ArrayTree<Node> //3 → 1 → 2
                        (
                            new Leaf(48), //3 → 1 → 2 → 0
                            new Leaf(410), //3 → 1 → 2 → 1
                            new Leaf(411) //3 → 1 → 2 → 2
                        )),
                        new Leaf(511) //3 → 1 → 3
                    )),
                    new Leaf(580) //3 → 2
                ))
            );

            var tree = new Tree<Node>
            {
                new Leaf(5), //0
                new Leaf(9), //1
                new TreeNode(new Tree<Node> //2
                {
                    new Leaf(7), //2 → 0
                    new TreeNode(new Tree<Node> //2 → 1
                    {
                        new Leaf(8), //2 → 1 → 0
                        new Leaf(10), //2 → 1 → 1
                        new Leaf(11) //2 → 1 → 2
                    }),
                    new Leaf(80) //2 → 2
                }),
                new TreeNode(new Tree<Node> //3
                {
                    new Leaf(57), //3 → 0
                    new TreeNode(new Tree<Node> //3 → 1
                    {
                        new Leaf(58), //3 → 1 → 0
                        new Leaf(510), //3 → 1 → 1
                        new TreeNode(new Tree<Node> //3 → 1 → 2
                        {
                            new Leaf(48), //3 → 1 → 2 → 0
                            new Leaf(410), //3 → 1 → 2 → 1
                            new Leaf(411) //3 → 1 → 2 → 2
                        }),
                        new Leaf(511) //3 → 1 → 3
                    }),
                    new Leaf(580) //3 → 2
                })
            };
            tree.Insert(new TreeIndex(3, 1, 2, 2), arrTree[2]);
            var ai = new TreeIndex(3, 1);
            var bi = new TreeIndex(3, 1, 2);
            var i = Interval<TreeIndex>.CC(bi, ai);
            var i2 = Interval<TreeIndex>.CO(null, ai);
            var i3 = Interval<TreeIndex>.PositiveRealsNoZero;
            var a = tree[ai];
            var b = tree[bi];
            var ste = tree.SubTree(i, false);
            var ste2 = tree.SubTree(i2, false);
            var ste3 = tree.SubTree(i3, false);
            var st = ste.ToTree(tree => new TreeNode(tree));
            var st2 = ste2.ToTree(tree => new TreeNode(tree));
            var st3 = ste3.ToTree(tree => new TreeNode(tree));
            var st4e = tree.SubTree((new TreeIndex(2, 1, 1), new TreeIndex(3, 1, 2, 2)), false);
            var st4 = st4e.ToTree(t => new TreeNode(t) { Indice = 5 });
            var st4l = st4.AllTreeLeafs().ToArray();
            var st4n = st4e.AllTreeNodes().ToArray();
        }

        public abstract class Node
        {
            public int Value { get; set; }
            public override string ToString() => Type + Value.ToString();
            public abstract string Type { get; }
        }
        public class Leaf : Node
        {
            public Leaf(int value) => Value = value;
            public override string Type => "Leaf";
        }
        public class TreeNode : Node, ITreeNode<Node>
        {
            public double Indice { get; set; }
            public override string Type => Indice.ToString() + "Node";

            public TreeNode(ITree<Node> children)
            {
                Children = children;
                Value = children.AllTreeLeafs().Sum(leaf => leaf.Value);
            }

            public ITree<Node> Children { get; }
        }
    }
}
