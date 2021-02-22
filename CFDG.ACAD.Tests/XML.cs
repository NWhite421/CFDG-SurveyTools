using NUnit.Framework;
using System;

namespace CFDG.API.Tests
{
    [TestFixture]
    public class XML
    {
        [Theory]
        [TestCase("general", "companyabbreviation", "CFDG")]
        [TestCase("autocad", "enableosnapz", true)]
        [TestCase("autocad", "novalue", null)]
        public void GetValues(string category, string key, object expected)
        {
            var value = API.XML.ReadValue(category, key);
            Assert.AreEqual(value, expected);
        }
    }
}
