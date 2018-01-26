using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Test
{
    [TestClass]
    public class Cpalls_UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void Test_CalculateBentchmark()
        {
            CpallsBusiness business = new CpallsBusiness(false);

            var list = new List<CutOffScoreEntity>()
            {
                new CutOffScoreEntity(){FromYear = 0,FromMonth = 0,ToYear = 4,ToMonth = 0,CutOffScore = 4},
                new CutOffScoreEntity(){FromYear = 4,FromMonth = 0,ToYear = 10,ToMonth = 0,CutOffScore = 10},
                new CutOffScoreEntity(){FromYear = 10,FromMonth = 0,ToYear = 14,ToMonth = 0,CutOffScore = 14},
                new CutOffScoreEntity(){FromYear = 14,FromMonth = 0,ToYear = 20,ToMonth = 0,CutOffScore = 20},
                new CutOffScoreEntity(){FromYear = 20,FromMonth = 0,ToYear = 24,ToMonth = 0,CutOffScore = 24},
            };
            Assert.AreEqual(4, CpallsBusiness.CalculateBenchmark(list, new DateTime(2012, 9, 21), "14-15"));
            Assert.AreEqual(10, CpallsBusiness.CalculateBenchmark(list, new DateTime(2010, 9, 21), "14-15"));

            Assert.AreEqual(10, CpallsBusiness.CalculateBenchmark(list, new DateTime(2010, 9, 20), "14-15"));
            Assert.AreEqual(10, CpallsBusiness.CalculateBenchmark(list, new DateTime(2010, 9, 1), "14-15"));

            Assert.AreEqual(14, CpallsBusiness.CalculateBenchmark(list, new DateTime(2004, 9, 21), "14-15"));
            Assert.AreEqual(14, CpallsBusiness.CalculateBenchmark(list, new DateTime(2004, 9, 20), "14-15"));
        }
    }
}
