using BenLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BenLibTest
{
    [TestClass]
    public class BenLibTest
    {
        [TestMethod]
        public void RangeTest()
        {
            var a = (IntRange)(6, 6 + 2) + (9, 9 + 1); //[6 ; 8[ ∪ [9 ; 10[
            var b = a * new IntRange(0, 4); //∅
            var c = b.Contains(a); //false
            var d = (IntRange)(null, 3) + (-5, 6); //]-∞ ; 6[
            d += (9, null); //]-∞ ; 6[ ∪ [9 ; +∞[
            d *= (-20, null); //[-20 ; 6[ ∪ [9 ; +∞[
            var e = ((IntRange)(0, 4)).Contains((0, 4)); //true
            var f = IntInterval.Invert((IntRange)(0, 4) + (15, 600)); //]-∞ ; 0[ ∪ [4 ; 15[ ∪ [600 ; +∞[
            var g = ((IntRange)(-5, 9) + (12, null)) / ((IntRange)(null, 8) + (20, 50)); //[8 ; 9[ ∪ [12 ; 20[ ∪ [50 ; +∞[
        }
    }
}
