using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShirobokovaPartnerLib.Models;

namespace ShirobokovaPartnerTests
{
    [TestClass]
    public class DiscountCalculatorTests
    {
        [TestMethod]
        public void DiscountPercent_ZeroSales_ShouldReturnZero()
        {
            var partner = new Partner { TotalSales = 0 };
            Assert.AreEqual(0, partner.DiscountPercent);
        }

        [TestMethod]
        public void DiscountPercent_SalesLessThan10000_ShouldReturnZero()
        {
            var partner = new Partner { TotalSales = 9999 };
            Assert.AreEqual(0, partner.DiscountPercent);
        }

        [TestMethod]
        public void DiscountPercent_Sales10000To50000_ShouldReturn5()
        {
            var partner = new Partner { TotalSales = 30000 };
            Assert.AreEqual(5, partner.DiscountPercent);
        }


        [TestMethod]
        public void DiscountPercent_Sales50000To300000_ShouldReturn10()
        {
            var partner = new Partner { TotalSales = 150000 };
            Assert.AreEqual(10, partner.DiscountPercent);
        }

        [TestMethod]
        public void DiscountPercent_SalesMoreThan300000_ShouldReturn15()
        {
            var partner = new Partner { TotalSales = 500000 };
            Assert.AreEqual(15, partner.DiscountPercent);
        }
    }
}