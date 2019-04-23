using static BenLib.Interval<decimal>;
using static BenLib.Ordinal<decimal>;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BenLib.Test
{
    [TestClass]
    public class BenLibTest
    {
        [TestMethod]
        public void RangeTest()
        {
            var times = Enumerable.Range(0, 10).Select(i => Eval()).ToArray();
            double av = times.Average(ts => ts.TotalMilliseconds);

            System.TimeSpan Eval()
            {
                var date = System.DateTime.Now;

                for (int i = 0; i < 100000; i++)
                {
                    var issss = new[] { NegativeInfinity, new BenLib.Ordinal<decimal>(0, 1) };
                    issss.OrderBy(i => i).ToArray();
                    var zz = CO(10, 96); //[10 ; 96[
                    var sss = new[] { CO(5, 9), OC(5, 9), Reals, OO(6, 8) }; // { [5 ; 9[ , ]5 ; 9] , ℝ , ]6 ; 8[ }
                    string str = sss[1].ToString(); //]5 ; 9]
                    var sssso = sss.OrderBy(r => r.Start).ToArray(); // { ℝ, [5 ; 9[ , ]5 ; 9] , ]6 ; 8[ }
                    var re = OC(0, PositiveInfinity) + CO(NegativeInfinity, 0);
                    string rs = re.ToString();
                    bool r = re.Contains(0);
                    var na = CC(5, 9); //[5 ; 9]
                    var nb = CO(0, 5); //[0 ; 5[
                    var nab = na * nb; //∅
                    var z = CC(0, 3) + CC(0, 1); //[0 ; 3]
                    var a = CO(6, 6 + 2) + CO(9, 9 + 1); //[6 ; 8[ ∪ [9 ; 10[
                    var b = a * CC(0, 4); //∅
                    bool c = b.Contains(a); //false
                    var d = CO(NegativeInfinity, 3) + OC(-5, 6); //]-∞ ; 6]
                    d += CC(9, PositiveInfinity); //]-∞ ; 6[ ∪ [9 ; +∞[
                    d *= CC(-20, PositiveInfinity); //[-20 ; 6] ∪ [9 ; +∞[
                    bool e = CC(0, 4).Contains(CC(0, 4)); //true
                    var f = (CO(0, 4) + CO(15, 600)).Invert; //]-∞ ; 0[ ∪ [4 ; 15[ ∪ [600 ; +∞[
                    var g = (CO(-5, 9) + CO(12, PositiveInfinity)) / (CO(NegativeInfinity, 8) + CO(20, 50)); //[8 ; 9[ ∪ [12 ; 20[ ∪ [50 ; +∞[
                }

                return System.DateTime.Now - date;
            }
        }
    }
}
