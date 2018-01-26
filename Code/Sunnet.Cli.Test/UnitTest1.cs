using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Reports.Model;

namespace Sunnet.Cli.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestMethod]
        public void TestGetRowByIndex()
        {
            YAxis yAxis = new YAxis();

            Assert.AreEqual(1, yAxis.Next, "init:1");
            Assert.AreEqual(2, yAxis.Next, "Next:+1");
            Assert.AreEqual(12, yAxis.Step(10), "Step:+10");
            Assert.AreEqual(13, yAxis.Next, "Next(Record Step):+1");
            Assert.AreEqual(11, yAxis.Step(-2), "Step Negative:-2");
            Assert.AreEqual(13, yAxis.GetByStep(2), "GetByStep:2");
            Assert.AreEqual(12, yAxis.Next, "GetByStep not record");
            Assert.AreEqual(10, yAxis.StepBack(2), "StepBack:2");
            Assert.AreEqual(12, yAxis.StepBack(-2), "StepBack:-2");
            yAxis.Reset();
            Assert.AreEqual(1, yAxis.Next, "Reset:1");
        }

        [TestMethod]
        public void TestGetColumnByIndex()
        {
            //  A  B  C  D  E  F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z
            // 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26
            XAxis xAxis = new XAxis();
            Assert.AreEqual("A", xAxis.Next, "Init: A");
            Assert.AreEqual("B", xAxis.Next, "Next: B");
            Assert.AreEqual("L", xAxis.Step(10), "Step 10: L");
            Assert.AreEqual("M", xAxis.Next, "Step Recorded");
            Assert.AreEqual("K", xAxis.Step(-2), "Step(-2)");
            Assert.AreEqual("M", xAxis.GetByStep(2), "GetByStep(2)");
            Assert.AreEqual("L", xAxis.Next, "GetByStep Not recorded");
            Assert.AreEqual("J", xAxis.StepBack(2), "StepBack(2)");
            Assert.AreEqual("M", xAxis.StepBack(-3), "StepBack(-3)");
            Assert.AreEqual("Y", xAxis.Step(12), "Step(12)");
            Assert.AreEqual("Z", xAxis.Next, "End:Z");
            Assert.AreEqual("AA", xAxis.Next, "Next:AA");
            Assert.AreEqual("AZ", xAxis.Step(25), "Second Round End:AZ" + xAxis.Raw);
            Assert.AreEqual("BA", xAxis.Next, "Thrid Round Start:BA");
            Assert.AreEqual("BZ", xAxis.Step(25), "Thrid Round End:BZ");
            xAxis.Reset();
            Assert.AreEqual("A", xAxis.Next, "Reset: A");
            Assert.AreEqual("ZZ", xAxis.Step(701), "End of Two bite: ZZ");
            Assert.AreEqual("AAA", xAxis.Next, "Start of three bite: AAA" + xAxis.Raw);
            Assert.AreEqual("AAZ", xAxis.Step(25), "AAZ:" + xAxis.Raw);
            Assert.AreEqual("ABZ", xAxis.Step(26), "ABZ:" + xAxis.Raw);
            Assert.AreEqual("AZZ", xAxis.Step(26 * 24), "AZZ:" + xAxis.Raw);
            Assert.AreEqual("BAA", xAxis.Next, "BAA:" + xAxis.Raw);
        }
    }
}
