using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sunnet.Framework.StringZipper;

namespace Sunnet.Cli.Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestZip()
        {
            var folder = @"D:\log\cli\Upload\Community\67";
            var zipfile = DateTime.Now.Ticks.ToString() + ".zip";
            var folderTo = @"D:\log\cli\Upload\Community\67\CircleDataExport_2015_06_25_16_39_01_550929E";
            CSharpCodeStringZipper.CreateZip(folder, zipfile, folderTo);
            Assert.AreEqual(true, File.Exists(Path.Combine(folder, zipfile)));
        }
    }
}
