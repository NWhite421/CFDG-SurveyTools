using NUnit.Framework;
using System;

namespace CFDG.API.Tests
{
    [TestFixture]
    public class XML
    {
        [Test]
        public void GetValues()
        {
            string value = API.XML.ReadValue("general", "companyabbreviation");
            Assert.AreEqual(value, "CFDG");
            value = API.XML.ReadValue("AUTOCAD", "EnableOSnapZ");
            Assert.AreEqual(value, "true");
            value = API.XML.ReadValue("AUTOCAD", "NonExistantValue");
            Assert.AreEqual(value, null);
        }
    }
}
