using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CFDG.API;

namespace APITests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string _ = INI.GetAppConfigSetting("API", "DevMode");
        }
    }
}
